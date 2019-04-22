'use strict';

require('mocha-sinon');
const chai = require('chai');
const sinon = require('sinon');
const sinonChai = require('sinon-chai');
const expect = chai.expect;
chai.use(sinonChai);

const compressor = require('../compress-message');
const Message = require('azure-iot-device').Message;

/*
The compressed and decompressed data below is based on the content message string.
The decompressedContentData is the below message in decimal and the compressedContentData
is the same message compressed.
const content = "this is a message.";
*/
const compressedContentData = Buffer.from([31, 139, 8, 0, 0, 0, 0, 0, 0, 10, 43, 201, 200, 44,
    86, 0, 162, 68, 133, 220, 212, 226, 226, 196, 244,
    84, 61, 0, 249, 210, 81, 69, 18, 0, 0, 0]);
const decompressedContentData = Buffer.from([116, 104, 105, 115, 32, 105, 115, 32, 
    97, 32, 109, 101, 115, 115, 97, 103, 101, 46]);

const emptyTestMsg = new Message();
const decompressedTestMsg = new Message(decompressedContentData);
console.log(JSON.stringify(decompressedTestMsg));
const compressedTestMsg = new Message(compressedContentData);

describe('calling compressor.compressMessage', () => {
    it('should log the size of the compressed and decompressed message');

    it('should pass a Message with no data through without modifying it', () => {
        compressor.compressMessage(emptyTestMsg, function(err, compressedMsgData){
            //Because we are comparing the headers, we have to get a copy of the original 
            //and then add the data from the function call to it
            const emptyTestMsgCopy = emptyTestMsg;
            emptyTestMsgCopy.data = compressedMsgData;  
            expect(emptyTestMsgCopy).to.equal(emptyTestMsg);
        });
    });

    it('should return a compressed buffer for message with data', function() {
        compressor.compressMessage(decompressedTestMsg, function(err, compressedMsgData){
            expect(compressedMsgData).to.deep.equal(compressedContentData);
        });
    });
});

describe('calling compressor.decompressMessage', () => {
    it('should pass a Message with no data through without modifying it', () => {
        compressor.decompressMessage(emptyTestMsg, function(err, compressedMsgData){
            //Because we are comparing the headers, we have to get a copy of the original 
            //and then add the data from the function call to it
            const emptyTestMsgCopy = emptyTestMsg;
            emptyTestMsgCopy.data = compressedMsgData;  
            expect(emptyTestMsgCopy).to.equal(emptyTestMsg);
        });
    });


    it('should return a decompressed buffer for message with data', () => {
        compressor.decompressMessage(compressedTestMsg, function(err, decompressedMsgData){
            console.log(decompressedMsgData);
            expect(decompressedMsgData).to.deep.equal(decompressedContentData);
        });
    });  
})

describe('calling compressor.compressMessage and compressor.decompressMessage in sequence', () => {
    it('should recreate the original message', () => {
        compressor.compressMessage(decompressedTestMsg, (err, compressedData) => {
            const compressedDataMsg = new Message(compressedData);
            compressor.decompressMessage(compressedDataMsg, (err, decompressedData) => {
                const decompressedDataMsg = new Message(decompressedData);
                expect(decompressedDataMsg).to.deep.equal(decompressedTestMsg);
            })
        })
    });
});