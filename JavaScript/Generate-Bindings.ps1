$protofile_name = "customers.proto"
$protoc = node_modules/grpc-tools/bin/protoc.exe

.\node_modules\grpc-tools\bin\protoc.exe -I ../Core/Protos `
       --js_out=import_style=commonjs:./ `
       --grpc-web_out=import_style=commonjs,mode=grpcwebtext:./ `
       $protofile_name
