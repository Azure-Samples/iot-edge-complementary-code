'use strict';

var Transport = require('azure-iot-device-mqtt').Mqtt;
var Client = require('azure-iot-device').ModuleClient;
var Message = require('azure-iot-device').Message;
const fs = require('fs');
const glob = require('glob');
const util = require('util');

const readFileAsync = util.promisify(fs.readFile);
const globAsync = util.promisify(glob);
const fromEnvironmentAsync = util.promisify(Client.fromEnvironment);

async function run() {

  let interval = 30000;
  let folder = "messages/";
  let prefix = "*.xml";
  let outputChannelName = "output"

  let client = await fromEnvironmentAsync(Transport);
 
  await client.open();
  console.log('client initialized');
 
  let path = folder + prefix;

  const done = false;

  while (!done) {
    let files = await globAsync(path);

    console.log(files.length);
    for (let i = 0; i < files.length; i++) {

      let data = await readFileAsync(files[i], {});
      let response = await client.sendOutputEvent(outputChannelName, new Message(data));
      await sleep(interval);
    }
  }
}

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

run();