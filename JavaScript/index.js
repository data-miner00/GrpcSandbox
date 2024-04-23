// Code is not working..
var { CustomerServiceClient } = require("./customers_grpc_web_pb");
var { NewCustomerRequest } = require("./customers_pb");
var { Timestamp } = require("google-protobuf/google/protobuf/timestamp_pb");

console.log("client side code");

var client = new CustomerServiceClient("localhost:5117");

client.getCustomerInfo({ userId: 1 }, {}, function (err, response) {
  if (err) {
    console.error(err);
  } else {
    console.log(response);
  }
});
