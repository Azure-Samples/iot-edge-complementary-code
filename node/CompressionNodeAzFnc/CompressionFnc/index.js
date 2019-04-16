const decompressMessage = require('./compress-message.js').decompressMessage;

module.exports = function (context, IoTHubMessage) {

  decompressMessage(Buffer.from(IoTHubMessage), function(err, data){
      if(err) {
        console.log(err);
      } else {
      var st = data.toString('utf8');
      context.log("Data after decompress:" + st);  
      context.bindings.outputBlob = data;
      context.done();
      }
    });
};
