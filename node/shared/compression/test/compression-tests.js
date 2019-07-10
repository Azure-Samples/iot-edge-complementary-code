'use strict';

require('mocha-sinon');
const chai = require('chai');
const sinon = require('sinon');
const sinonChai = require('sinon-chai');
const expect = chai.expect;
chai.use(sinonChai);

const compressor = require('../compress-message');

/*
The compressed and decompressed data below is based on the content message string.
The decompressedContentData is the below message in decimal and the compressedContentData
is the same message compressed.
const content = "this is a message.";
*/
const emptyContentData = Buffer.from([]);
const compressedContentData = Buffer.from([31, 139, 8, 0, 0, 0, 0, 0, 0, 10, 43, 201, 200, 44,
    86, 0, 162, 68, 133, 220, 212, 226, 226, 196, 244,
    84, 61, 0, 249, 210, 81, 69, 18, 0, 0, 0]);
const decompressedContentData = Buffer.from([116, 104, 105, 115, 32, 105, 115, 32, 
    97, 32, 109, 101, 115, 115, 97, 103, 101, 46]);

describe('calling compressor.compressMessage', () => {
    
    it('should pass a empty data through without modifying it', () => {
        compressor.compressMessage(emptyContentData, function(err, compressedData){
            //Because we are comparing the headers, we have to get a copy of the original 
            //and then add the data from the function call to it
            expect(compressedData).to.equal(emptyContentData);
        });
    });

    it('should return a compressed buffer of the data', function() {
        compressor.compressMessage(decompressedContentData, function(err, compressedData){
            expect(compressedData).to.deep.equal(compressedContentData);
        });
    });
});

describe('calling compressor.decompressMessage', () => {
    it('should pass empty data through without modifying it', () => {
        compressor.decompressMessage(emptyContentData, function(err, decompressedData){
            //Because we are comparing the headers, we have to get a copy of the original 
            //and then add the data from the function call to it 
            expect(decompressedData).to.equal(emptyContentData);
        });
    });


    it('should return a decompressed buffer of the data', () => {
        compressor.decompressMessage(compressedContentData, function(err, decompressedData){
            console.log(decompressedData);
            expect(decompressedData).to.deep.equal(decompressedContentData);
        });
    });  
})

describe('calling compressor.compressMessage and compressor.decompressMessage in sequence', () => {
    it('should recreate the original message', () => {
        compressor.compressMessage(decompressedContentData, (err, compressedData) => {
            compressor.decompressMessage(compressedData, (err, decompressedData) => {
                expect(decompressedData).to.deep.equal(decompressedContentDta);
            })
        })
    });
});