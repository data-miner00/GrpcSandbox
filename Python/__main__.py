import grpc

from customers_pb2 import CustomerLookupRequest
from customers_pb2_grpc import CustomerServiceStub
from google.protobuf.timestamp_pb2 import Timestamp


def eager_create_timestamp_now() -> Timestamp:
    now = Timestamp()
    now.GetCurrentTime()
    return now

if __name__ == "__main__":
    print("hello from gRPC Python.")
    userId = 2

    channel = grpc.insecure_channel("localhost:5117")
    stub = CustomerServiceStub(channel)


    param = CustomerLookupRequest(UserId=userId)

    response = stub.GetCustomerInfo(param)

    print(response)
