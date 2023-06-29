using System;
using System.Diagnostics;
using System.IO;

public class TimestampedTextWriterTraceListener : TextWriterTraceListener
{
    public TimestampedTextWriterTraceListener(Stream stream) : base(stream)
    {
    }

    public TimestampedTextWriterTraceListener(string fileName) : base(fileName)
    {
    }

    public TimestampedTextWriterTraceListener(TextWriter writer) : base(writer)
    {
    }

    public override void Write(string message)
    {
        string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        base.Write($"[{timestamp}] {message}");
    }

    public override void WriteLine(string message)
    {
        string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        base.WriteLine($"[{timestamp}] {message}");
    }
}