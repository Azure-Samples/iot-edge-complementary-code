'use strict';

const Transport = require('azure-iot-device-mqtt').Mqtt;
const Client    = require('azure-iot-device').ModuleClient;
const Message   = require('azure-iot-device').Message;
const compressMessage = require('../../../shared/compress-message.js').compressMessage;
const decompressMessage = require('../../../shared/compress-message.js').decompressMessage;

/*
compressMessage and decompressMessage are the routes for the deployment JSON
*/

const COMPRESS_MESSAGE = 'compressMessage';
const COMPRESS_MESSAGE_OUTPUT =  'compressMessageOutput';
const DECOMPRESS_MESSAGE = 'decompressMessage';
const DECOMPRESS_MESSAGE_OUTPUT = 'decompressMessageOutput';

Client.fromEnvironment(Transport, function (err, client) {
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
        console.log('IoT Hub compress module client initialized');
        // Act on messages to (de)compress to the module.
        client.on('inputMessage', function (inputName, msg) {
          processMessage(client, inputName, msg);
        });
      }
    });
  }
});

function processMessage(client, inputName, msg) {
  client.complete(msg, printResultFor('Receiving message'));

  /*IoT Message Error Handling*/
  if(!msg){ console.log(err) ; }; 

  /*Operate on IoT Message*/
  const outputMsg = new Message(msg.getBytes());
  let msgName = undefined;
  /* */
  const callback = function(err, data){
    (!err) ? outputMsg.data = data : console.log(err);

    console.debug("Sending message through IoT Hub with data: "+ outputMsg.getBytes());
    
    client.sendOutputEvent(msgName, outputMsg, printResultFor('Sending received message'));
  }

  switch(inputName){
    case COMPRESS_MESSAGE:
        //msgName used in callback
        msgName = COMPRESS_MESSAGE_OUTPUT;    
        compressMessage(outputMsg.getData(), callback);
        break;
    case DECOMPRESS_MESSAGE:
        //msgName used in callback
        msgName = DECOMPRESS_MESSAGE_OUTPUT;            
        decompressMessage(outputMsg.getData(), callback);
        break;
    default:
        console.debug("Route message name is not supported.")
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
}
