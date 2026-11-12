
using System;
using System.Collections.Generic;

public class Ambiente
{
    public int id;
    public string nome;
    public Queue<Log> logs;

    public Ambiente()
    {
        logs = new Queue<Log>();
    }

    public void registrarLog(Log log)
    {
        if (logs.Count >= 100)
            logs.Dequeue();

        logs.Enqueue(log);
    }
}
