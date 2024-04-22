$protofile_name = "customers.proto"

python -m grpc.tools.protoc `
       -I ../Core/Protos/ `
       --python_out ./ `
       --grpc_python_out ./ `
       --pyi_out ./ `
       $protofile_name