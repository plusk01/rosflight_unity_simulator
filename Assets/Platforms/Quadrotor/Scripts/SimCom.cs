using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class SimCom : MonoBehaviour {

    [HeaderAttribute("External Address")]
    public string ipAddress = "127.0.0.1";
    public int port = 2908;

    // UDP socket for send / receive to ROS bridge
    private UdpClient socket = new UdpClient();

    // Endpoint (IP, port) that is the address to the external ROS bridge
    private IPEndPoint simEP;

    // ------------------------------------------------------------------------

    public bool SendIMU(long timestampTicks, Vector3 accel, Vector3 gyro)
    {
        byte[] imuRaw = PackIMU(timestampTicks, accel, gyro);

        socket.Send(imuRaw, imuRaw.Length, simEP);
        return true;
    }

    // ------------------------------------------------------------------------
    // Private Methods
    // ------------------------------------------------------------------------

    // Use this for initialization
    void Start()
    {
        // "Bind" to endpoint
        simEP = new IPEndPoint(IPAddress.Parse(ipAddress), port);
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

        byte[] bytes = new byte[2*sizeof(int) + 3*sizeof(float) + 3*sizeof(float)];

        int offset = 0;
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

    void TicksToTime(long ticks, out int secs, out int nsecs)
    {
        // In C#, 1 tick == 100 ns (https://stackoverflow.com/a/386356/2392520)
        double s = TimeSpan.FromTicks(ticks).TotalSeconds;

        secs = (int) Math.Truncate(s);
        nsecs = (int) (Math.Truncate((s-secs)*1e7)*1e2);
    }
}
