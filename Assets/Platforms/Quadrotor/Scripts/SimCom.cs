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

    public bool SendIMU(double timestamp, Vector3 accel, Vector3 gyro)
    {
        byte[] imuRaw = PackIMU(timestamp, accel, gyro);

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

    byte[] PackIMU(double timestamp, Vector3 accel, Vector3 gyro)
    {
        byte[] bytes = new byte[sizeof(double) + 3*sizeof(float) + 3*sizeof(float)];

        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(timestamp), 0, bytes, offset, sizeof(double)); offset += sizeof(double);
        Buffer.BlockCopy(BitConverter.GetBytes(accel.x), 0, bytes, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(accel.y), 0, bytes, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(accel.z), 0, bytes, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(gyro.x), 0, bytes, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(gyro.y), 0, bytes, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(gyro.z), 0, bytes, offset, sizeof(float)); offset += sizeof(float);

        return bytes;
    }
}
