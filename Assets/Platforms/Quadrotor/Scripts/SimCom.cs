using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class SimCom : MonoBehaviour {

    public delegate void MotorCmdHandler(float[] motors);
    public event MotorCmdHandler OnMotorCmd;

    [HeaderAttribute("Remote Host")]
    public string remoteIP = "127.0.0.1";
    public int remotePort = 2908;

    [HeaderAttribute("Bind Host")]
    public string bindIP = "127.0.0.1";
    public int bindPort = 2909;

    // UDP socket for send / receive to UnityBridge
    private UdpClient socket;

    // Endpoint (IP, port) that is the address to the remote UnityBridge
    private IPEndPoint remoteEP;

    // Endpoint (IP, port) of the bind address (local)
    private IPEndPoint bindEP;

    // Thread for receiving data from UnityBridge
    private Thread rxThread;

    private enum Message : byte {
        SimConfig = 0x00,
        VehConfig = 0x01,
        IMU = 0x02,
        MotorCmd = 0x03
    }

    // ------------------------------------------------------------------------

    public bool SendIMU(long timestampTicks, Vector3 accel, Vector3 gyro)
    {
        byte[] imuRaw = PackIMU(timestampTicks, accel, gyro);

        socket.Send(imuRaw, imuRaw.Length, remoteEP);
        return true;
    }

    // ------------------------------------------------------------------------
    // Private Methods
    // ------------------------------------------------------------------------

    // Use this for initialization
    void Start()
    {
        // Create endpoints
        bindEP = new IPEndPoint(IPAddress.Parse(bindIP), bindPort);
        remoteEP = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);

        // Bind UDP socket to local endpoint
        socket = new UdpClient(bindEP);

        // Start rx thread
        rxThread = new Thread(new ThreadStart(ReceiveData));
        rxThread.IsBackground = true;
        rxThread.Start();
    }

    // ------------------------------------------------------------------------
    
    // Update is called once per frame
    void Update()
    {
        
    }

    // ------------------------------------------------------------------------

    byte[] PackIMU(long timestampTicks, Vector3 accel, Vector3 gyro)
    {
        int secs, nsecs;
        TicksToTime(timestampTicks, out secs, out nsecs);

        byte[] bytes = new byte[1 + 2*sizeof(int) + 3*sizeof(float) + 3*sizeof(float)];

        int offset = 0;
        bytes[0] = (byte)Message.IMU; offset += 1;
        Buffer.BlockCopy(BitConverter.GetBytes(secs), 0, bytes, offset, sizeof(int)); offset += sizeof(int);
        Buffer.BlockCopy(BitConverter.GetBytes(nsecs), 0, bytes, offset, sizeof(int)); offset += sizeof(int);
        Buffer.BlockCopy(BitConverter.GetBytes(accel.x), 0, bytes, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(accel.y), 0, bytes, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(accel.z), 0, bytes, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(gyro.x), 0, bytes, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(gyro.y), 0, bytes, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(gyro.z), 0, bytes, offset, sizeof(float)); offset += sizeof(float);

        return bytes;
    }

    // ------------------------------------------------------------------------

    void ReceiveData()
    {
        while (true) {
            // This will be filled with the address of where the data came from
            IPEndPoint senderEP = new IPEndPoint(IPAddress.Any, 0);

            // wait for a packet to be received (blocking)
            byte[] buffer = socket.Receive(ref senderEP);

            // The first byte is the msg type
            byte[] data = buffer.Skip(1).ToArray();

            switch (buffer[0]) {
                case (byte)Message.SimConfig:
                    break;
                case (byte)Message.VehConfig:
                    break;
                case (byte)Message.MotorCmd:
                    ParseMotorCmd(data);
                    break;
                default: break; // unrecognized msg type
            }

        }
    }

    // ------------------------------------------------------------------------

    void TicksToTime(long ticks, out int secs, out int nsecs)
    {
        // In C#, 1 tick == 100 ns (https://stackoverflow.com/a/386356/2392520)
        double s = TimeSpan.FromTicks(ticks).TotalSeconds;

        secs = (int) Math.Truncate(s);
        nsecs = (int) (Math.Truncate((s-secs)*1e7)*1e2);
    }

    // ------------------------------------------------------------------------

    void ParseMotorCmd(byte[] data)
    {
        // Calculate the number of motor commands that were sent
        int numMotors = data.Length / sizeof(float);
        float[] motors = new float[numMotors];

        for (int i=0; i<numMotors; ++i) {
            motors[i] = BitConverter.ToSingle(data, i*sizeof(float));
        }

        OnMotorCmd(motors);
    }
}
