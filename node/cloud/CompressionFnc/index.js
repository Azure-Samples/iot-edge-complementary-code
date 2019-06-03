const decompressMessage = require('../../shared/compress-message.js').decompressMessage;

module.exports = (context, IoTHubMessage) => {

  if (context.bindingData.properties["compression"] == "gzip") {
    decompressMessage(Buffer.from(IoTHubMessage), (err, data) => {
      if (err) {
        console.log(err);
      } else {
        var st = data.toString('utf8');
        context.log("Decompressed message: " + st);
        context.bindings.outputBlob = data;
        context.done();      
      }
    });
  }
  else {
    console.log("Received uncompressed: " + IoTHubMessage.toString('utf8'));
    context.bindings.outputBlob = IoTHubMessage;
    context.done();
    
  }
  
}
