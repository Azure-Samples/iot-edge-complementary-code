'use strict';

var Transport = require('azure-iot-device-mqtt').Mqtt;
var Client = require('azure-iot-device').ModuleClient;
var Message = require('azure-iot-device').Message;
var Fs = require('fs');
var Path = require('path');


async function run() {

  let interval = 30000;
  let folder = "messages";
  let prefix = "*.xml";
  let outputChannelName = "output"

  Client.fromEnvironment(Transport, (err, client) => {
    if (err) {
      throw err;
    } else {
      client.on('error', function (err) {
        throw err;
      });

      // connect to the Edge instance
      client.open(function (err) {
        if (err) {
          throw err;
        } else {
          console.log('IoT Hub module client initialized');

          let path = folder + prefix;

          const done = false;

          while (!done) {
            let files = await globAsync(path);
            for (let i = 0; i < files.length; i++) {

              let data = await readFileAsync(files[i], {});
              let response = await client.sendOutputEvent(outputChannelName, new Message(data));
              await sleep(interval);
            }
          }

        }
      });
    }
  });
}

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}


// This function just pipes the messages without any change.
function pipeMessage(client, inputName, msg) {
  client.complete(msg, printResultFor('Receiving message'));

  if (inputName === 'input1') {
    var message = msg.getBytes().toString('utf8');
    if (message) {
      var outputMsg = new Message(message);
      client.sendOutputEvent('output1', outputMsg, printResultFor('Sending received message'));
    }
  }
}

// Helper function to print results in the console
function printResultFor(op) {
  return function printResult(err, res) {
    if (err) {
      console.log(op + ' error: ' + err.toString());
    }
    if (res) {
      console.log(op + ' status: ' + res.constructor.name);
    }
  };
}

run();