
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class Cadastro
{
    public List<Usuario> usuarios;
    public List<Ambiente> ambientes;

    public Cadastro()
    {
        usuarios = new List<Usuario>();
        ambientes = new List<Ambiente>();
    }

    public void adicionarUsuario(Usuario usuario)
    {
        usuarios.Add(usuario);
    }

    public bool removerUsuario(Usuario usuario)
    {
        if (usuario == null || usuario.ambientes.Count > 0)
            return false;

        return usuarios.Remove(usuario);
    }

    public Usuario pesquisarUsuario(Usuario usuario)
    {
        return usuarios.FirstOrDefault(u => u.id == usuario.id);
    }

    public void adicionarAmbiente(Ambiente ambiente)
    {
        ambientes.Add(ambiente);
    }

    public bool removerAmbiente(Ambiente ambiente)
    {
        if (ambiente == null)
            return false;

        foreach (Usuario usuario in usuarios)
        {
            usuario.revogarPermissao(ambiente);
        }

        return ambientes.Remove(ambiente);
    }

    public Ambiente pesquisarAmbiente(Ambiente ambiente)
    {
        return ambientes.FirstOrDefault(a => a.id == ambiente.id);
    }

    public void upload()
    {
        Dados dados = new Dados();

        foreach (Usuario usuario in usuarios)
        {
            dados.usuarios.Add(new UsuarioDados
            {
                id = usuario.id,
                nome = usuario.nome,
                ambientes = usuario.ambientes.Select(a => a.id).ToList()
            });
        }

        foreach (Ambiente ambiente in ambientes)
        {
            AmbienteDados ambienteDados = new AmbienteDados();

            ambienteDados.id = ambiente.id;
            ambienteDados.nome = ambiente.nome;

            foreach (Log log in ambiente.logs)
            {
                ambienteDados.logs.Add(new LogDados
                {
                    dtAcesso = log.dtAcesso,
                    idUsuario = log.usuario.id,
                    tipoAcesso = log.tipoAcesso
                });
            }

            dados.ambientes.Add(ambienteDados);
        }

        string json = JsonSerializer.Serialize(dados,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText("dados.json", json);
    }

    public void download()
    {
        if (!File.Exists("dados.json"))
            return;

        string json = File.ReadAllText("dados.json");

        Dados dados = JsonSerializer.Deserialize<Dados>(json);

        usuarios.Clear();
        ambientes.Clear();

        foreach (AmbienteDados ambienteDados in dados.ambientes)
        {
            Ambiente ambiente = new Ambiente();

            ambiente.id = ambienteDados.id;
            ambiente.nome = ambienteDados.nome;

            ambientes.Add(ambiente);
        }

        foreach (UsuarioDados usuarioDados in dados.usuarios)
        {
            Usuario usuario = new Usuario();

            usuario.id = usuarioDados.id;
            usuario.nome = usuarioDados.nome;

            usuarios.Add(usuario);
        }

        foreach (UsuarioDados usuarioDados in dados.usuarios)
        {
            Usuario usuario = pesquisarUsuario(
                new Usuario { id = usuarioDados.id });

            foreach (int idAmbiente in usuarioDados.ambientes)
            {
                Ambiente ambiente = pesquisarAmbiente(
                    new Ambiente { id = idAmbiente });

                if (ambiente != null)
                    usuario.concederPermissao(ambiente);
            }
        }

        foreach (AmbienteDados ambienteDados in dados.ambientes)
        {
            Ambiente ambiente = pesquisarAmbiente(
                new Ambiente { id = ambienteDados.id });

            foreach (LogDados logDados in ambienteDados.logs)
            {
                Usuario usuario = pesquisarUsuario(
                    new Usuario { id = logDados.idUsuario });

                if (usuario != null)
                {
                    Log log = new Log();

                    log.dtAcesso = logDados.dtAcesso;
                    log.usuario = usuario;
                    log.tipoAcesso = logDados.tipoAcesso;

                    ambiente.registrarLog(log);
                }
            }
        }
    }

    // Classes auxiliares para persistência

    public class Dados
    {
        public List<UsuarioDados> usuarios { get; set; } = new List<UsuarioDados>();
        public List<AmbienteDados> ambientes { get; set; } = new List<AmbienteDados>();
    }

    public class UsuarioDados
    {
        public int id { get; set; }
        public string nome { get; set; }
        public List<int> ambientes { get; set; } = new List<int>();
    }

    public class AmbienteDados
    {
        public int id { get; set; }
        public string nome { get; set; }
        public List<LogDados> logs { get; set; } = new List<LogDados>();
    }

    public class LogDados
    {
        public DateTime dtAcesso { get; set; }
        public int idUsuario { get; set; }
        public bool tipoAcesso { get; set; }
    }
}
