
using System;
using System.Linq;

class Program
{
    static Cadastro cadastro = new Cadastro();

    static void Main(string[] args)
    {
        cadastro.download();

        int opcao;

        do
        {
            Console.WriteLine("\n===== CONTROLE DE ACESSOS =====");
            Console.WriteLine("0 - Sair");
            Console.WriteLine("1 - Cadastrar ambiente");
            Console.WriteLine("2 - Consultar ambiente");
            Console.WriteLine("3 - Excluir ambiente");
            Console.WriteLine("4 - Cadastrar usuario");
            Console.WriteLine("5 - Consultar usuario");
            Console.WriteLine("6 - Excluir usuario");
            Console.WriteLine("7 - Conceder permissao");
            Console.WriteLine("8 - Revogar permissao");
            Console.WriteLine("9 - Registrar acesso");
            Console.WriteLine("10 - Consultar logs");

            Console.Write("Opcao: ");
            opcao = LerInt();

            switch (opcao)
            {
                case 1:
                    CadastrarAmbiente();
                    break;

                case 2:
                    ConsultarAmbiente();
                    break;

                case 3:
                    ExcluirAmbiente();
                    break;

                case 4:
                    CadastrarUsuario();
                    break;

                case 5:
                    ConsultarUsuario();
                    break;

                case 6:
                    ExcluirUsuario();
                    break;

                case 7:
                    ConcederPermissao();
                    break;

                case 8:
                    RevogarPermissao();
                    break;

                case 9:
                    RegistrarAcesso();
                    break;

                case 10:
                    ConsultarLogs();
                    break;

                case 0:
                    cadastro.upload();
                    Console.WriteLine("Dados salvos. Encerrando...");
                    break;

                default:
                    Console.WriteLine("Opcao invalida!");
                    break;
            }

        } while (opcao != 0);
    }

    static int LerInt()
    {
        int valor;

        while (!int.TryParse(Console.ReadLine(), out valor))
        {
            Console.Write("Digite um numero valido: ");
        }

        return valor;
    }

    static Ambiente SelecionarAmbiente()
    {
        Console.Write("ID do ambiente: ");
        int id = LerInt();

        return cadastro.pesquisarAmbiente(new Ambiente { id = id });
    }

    static Usuario SelecionarUsuario()
    {
        Console.Write("ID do usuario: ");
        int id = LerInt();

        return cadastro.pesquisarUsuario(new Usuario { id = id });
    }

    static void CadastrarAmbiente()
    {
        Ambiente ambiente = new Ambiente();

        Console.Write("ID: ");
        ambiente.id = LerInt();

        if (cadastro.pesquisarAmbiente(ambiente) != null)
        {
            Console.WriteLine("ID ja cadastrado!");
            return;
        }

        Console.Write("Nome: ");
        ambiente.nome = Console.ReadLine();

        cadastro.adicionarAmbiente(ambiente);

        Console.WriteLine("Ambiente cadastrado!");
    }

    static void ConsultarAmbiente()
    {
        Ambiente ambiente = SelecionarAmbiente();

        if (ambiente == null)
        {
            Console.WriteLine("Ambiente nao encontrado!");
            return;
        }

        Console.WriteLine("ID: " + ambiente.id);
        Console.WriteLine("Nome: " + ambiente.nome);
        Console.WriteLine("Quantidade de logs: " + ambiente.logs.Count);
    }

    static void ExcluirAmbiente()
    {
        Ambiente ambiente = SelecionarAmbiente();

        if (cadastro.removerAmbiente(ambiente))
            Console.WriteLine("Ambiente removido!");
        else
            Console.WriteLine("Ambiente nao encontrado!");
    }

    static void CadastrarUsuario()
    {
        Usuario usuario = new Usuario();

        Console.Write("ID: ");
        usuario.id = LerInt();

        if (cadastro.pesquisarUsuario(usuario) != null)
        {
            Console.WriteLine("ID ja cadastrado!");
            return;
        }

        Console.Write("Nome: ");
        usuario.nome = Console.ReadLine();

        cadastro.adicionarUsuario(usuario);

        Console.WriteLine("Usuario cadastrado!");
    }

    static void ConsultarUsuario()
    {
        Usuario usuario = SelecionarUsuario();

        if (usuario == null)
        {
            Console.WriteLine("Usuario nao encontrado!");
            return;
        }

        Console.WriteLine("ID: " + usuario.id);
        Console.WriteLine("Nome: " + usuario.nome);
        Console.WriteLine("Permissoes:");

        foreach (Ambiente ambiente in usuario.ambientes)
        {
            Console.WriteLine(ambiente.id + " - " + ambiente.nome);
        }
    }

    static void ExcluirUsuario()
    {
        Usuario usuario = SelecionarUsuario();

        if (cadastro.removerUsuario(usuario))
            Console.WriteLine("Usuario removido!");
        else
            Console.WriteLine("Usuario nao encontrado ou possui permissoes!");
    }

    static void ConcederPermissao()
    {
        Ambiente ambiente = SelecionarAmbiente();
        Usuario usuario = SelecionarUsuario();

        if (ambiente == null || usuario == null)
        {
            Console.WriteLine("Ambiente ou usuario nao encontrado!");
            return;
        }

        if (usuario.concederPermissao(ambiente))
            Console.WriteLine("Permissao concedida!");
        else
            Console.WriteLine("Usuario ja possui essa permissao!");
    }

    static void RevogarPermissao()
    {
        Ambiente ambiente = SelecionarAmbiente();
        Usuario usuario = SelecionarUsuario();

        if (ambiente == null || usuario == null)
        {
            Console.WriteLine("Ambiente ou usuario nao encontrado!");
            return;
        }

        if (usuario.revogarPermissao(ambiente))
            Console.WriteLine("Permissao revogada!");
        else
            Console.WriteLine("Usuario nao possui essa permissao!");
    }

    static void RegistrarAcesso()
    {
        Ambiente ambiente = SelecionarAmbiente();
        Usuario usuario = SelecionarUsuario();

        if (ambiente == null || usuario == null)
        {
            Console.WriteLine("Ambiente ou usuario nao encontrado!");
            return;
        }

        bool autorizado = usuario.ambientes.Contains(ambiente);

        Log log = new Log(usuario, autorizado);

        ambiente.registrarLog(log);

        if (autorizado)
            Console.WriteLine("ACESSO AUTORIZADO!");
        else
            Console.WriteLine("ACESSO NEGADO!");
    }

    static void ConsultarLogs()
    {
        Ambiente ambiente = SelecionarAmbiente();

        if (ambiente == null)
        {
            Console.WriteLine("Ambiente nao encontrado!");
            return;
        }

        Console.WriteLine("\n1 - Todos");
        Console.WriteLine("2 - Autorizados");
        Console.WriteLine("3 - Negados");
        Console.Write("Filtro: ");

        int filtro = LerInt();

        foreach (Log log in ambiente.logs)
        {
            if (filtro == 1 ||
                (filtro == 2 && log.tipoAcesso) ||
                (filtro == 3 && !log.tipoAcesso))
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine("Data: " + log.dtAcesso);
                Console.WriteLine("Usuario: " + log.usuario.nome);
                Console.WriteLine("ID: " + log.usuario.id);

                if (log.tipoAcesso)
                    Console.WriteLine("Resultado: AUTORIZADO");
                else
                    Console.WriteLine("Resultado: NEGADO");
            }
        }
    }
}
