'use strict';

const fs = require('fs');
const chai = require('chai');
const expect = chai.expect;
const compressor = require('../compress-message');

const emptyContentData = Buffer.from([]);
const compressedContentData = fs.readFileSync('test/1.xml.gz');
const decompressedContentData = fs.readFileSync('test/1.xml');

describe('calling compressMessage', () => {
   
    it('should pass a empty data through without modifying it', () => {
        compressor.compressMessage(emptyContentData, function(err, compressedData){
            expect(compressedData).to.equal(emptyContentData);
        });
    });

});

describe('calling decompressMessage', () => {
    it('should pass empty data through without modifying it', () => {
        compressor.decompressMessage(emptyContentData, function(err, decompressedData){
            expect(decompressedData).to.equal(emptyContentData);
        });
    });


    it('should decompressed an externally gzipped data', () => {
        compressor.decompressMessage(compressedContentData, function(err, decompressedData){
            expect(decompressedData).to.deep.equal(decompressedContentData);
        });
    });  
})

describe('calling compressMessage and decompressMessage in sequence', () => {
    it('should recreate the original message', () => {
        compressor.compressMessage(decompressedContentData, (err, compressedData) => {
            compressor.decompressMessage(compressedData, (err, decompressedData) => {
                expect(decompressedData).to.deep.equal(decompressedContentData);
            })
        })
    });
});