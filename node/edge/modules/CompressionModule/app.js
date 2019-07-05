'use strict';

const Transport = require('azure-iot-device-mqtt').Mqtt;
const Client = require('azure-iot-device').ModuleClient;
const Message = require('azure-iot-device').Message;
const compressMessage = require('compression').compressMessage;
const decompressMessage = require('compression').decompressMessage;


/*
compressMessage and decompressMessage are the routes for the deployment JSON
*/

const COMPRESS_MESSAGE = 'compressMessage';
const COMPRESS_MESSAGE_OUTPUT = 'compressMessageOutput';
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

function processMessage(client, inputName, message) {

  /*IoT Message Error Handling*/
  if (!message) {
    console.log(err);
    return;
  };

  client.complete(message);

  let messageBytes = message.getData();

  if (messageBytes.length == 0) {
    client.sendOutputEvent(COMPRESS_MESSAGE_OUTPUT, message);

    console.log("Message had no body and was passed as-is.");
    return;
  }

  switch (inputName) {
    case COMPRESS_MESSAGE:
      console.debug("Received message with body size: " + messageBytes.length);
      
      compressMessage(messageBytes, (err, compressedData) => {
        if (err) {
          console.log(err);
          return;
        }

        const outputMsg = new Message(compressedData);

        // copy all properties to new message
        message.properties.propertyList.forEach(element => {
          outputMsg.properties.add(element.key, element.value);
        });

        // add a new property indicating this message is compressed with gzip to allow backend to handle compressed and non-compressed messages
        outputMsg.properties.add("compression", "gzip");

        client.sendOutputEvent(COMPRESS_MESSAGE_OUTPUT, outputMsg);

        console.log("Sent compressed message with body size: " + outputMsg.getBytes().length);
      });
      break;
    case DECOMPRESS_MESSAGE:

      if (message.properties["compression"] == "gzip") {

        let compressedData = message.getData();
        console.debug("Received compressed message with body size: " + messageBytes.length);

        decompressMessage(messageBytes, (err, data) => {
          if (err) {
            console.log(err);
            return;
          }

          const outputMsg = new Message(data);

          message.properties.propertyList.forEach(element => {
            outputMsg.properties.add(element.key, element.value);
          });

          client.sendOutputEvent(DECOMPRESS_MESSAGE_OUTPUT, outputMsg);
          console.debug("Sent decompressed message with body size: " + outputMsg.getBytes());

        });
      }
      else {
        client.sendOutputEvent(DECOMPRESS_MESSAGE_OUTPUT, message);
        console.log("Message was not compressed and was passed as-is.");
      }
      break;

    default:
      console.log("Route message name is not supported.")
  }
}


