
using System;
using System.Collections.Generic;

public class Usuario
{
    public int id;
    public string nome;
    public List<Ambiente> ambientes;

    public Usuario()
    {
        ambientes = new List<Ambiente>();
    }

    public bool concederPermissao(Ambiente ambiente)
    {
        if (ambientes.Contains(ambiente))
            return false;

        ambientes.Add(ambiente);
        return true;
    }

    public bool revogarPermissao(Ambiente ambiente)
    {
        return ambientes.Remove(ambiente);
    }
}
