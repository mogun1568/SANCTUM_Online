protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

START ../../../SANCTUM_Server/PacketGenerator/bin/Debug/PacketGenerator.exe ./Protocol.proto
XCOPY /Y Protocol.cs "../../../SANCTUM_Client/Assets/Scripts/Packet"
XCOPY /Y Protocol.cs "../../../SANCTUM_Server/Server/Packet"
XCOPY /Y ClientPacketManager.cs "../../../SANCTUM_Client/Assets/Scripts/Packet"
XCOPY /Y ServerPacketManager.cs "../../../SANCTUM_Server/Server/Packet"