For an introduction to building an IoT Edge Module in Node.js, refer to [Tutorial: Develop and deploy a Node.js IoT Edge module for Linux devices](https://docs.microsoft.com/en-us/azure/iot-edge/tutorial-node-module).


As of version npm 2.0.0, a path to a local directory that contains a package can be used as in the dependencies section of package.json using the following syntax:

``
{
  "name": "baz",
  "dependencies": {
    "bar": "file:../foo/bar"
  }
}
``