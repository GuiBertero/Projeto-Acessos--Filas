
using System;

public class Log
{
    public DateTime dtAcesso;
    public Usuario usuario;
    public bool tipoAcesso;

    public Log()
    {
    }

    public Log(Usuario usuario, bool tipoAcesso)
    {
        dtAcesso = DateTime.Now;
        this.usuario = usuario;
        this.tipoAcesso = tipoAcesso;
    }
}
