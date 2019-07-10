'use strict';

const zlib = require('zlib');

function compressMessage(msgBody, callback) {
  if (msgBody && msgBody.length > 0) {
    zlib.gzip(msgBody, callback);
  } else {
    //If message is empty, pass through
    callback(null, msgBody);
  }
}

function decompressMessage(msgBody, callback) {
  if (msgBody && msgBody.length > 0) {
    zlib.gunzip(msgBody, callback);
  } else {
    //If message is empty, pass through
    callback(null, msgBody);
  }
}

module.exports = {
  compressMessage,
  decompressMessage
}