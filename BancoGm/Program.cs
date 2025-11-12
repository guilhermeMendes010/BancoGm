using System;
using MySql.Data.MySqlClient;

public class Program
{
    public static void Main(string[] args)
    {
        string conexao = "server=127.0.0.1;port=3307;database=bancovg;user=root;password=senac";
        int escolhaMenuInicial = 0;

        do
        {
            MenuInicial(ref escolhaMenuInicial);

            if (escolhaMenuInicial == 0)
            {
                CadastrarUsuario(conexao);
            }
            else if (escolhaMenuInicial == 1)
            {
                FazerLogin(conexao);
            }

            Console.Clear();

        } while (escolhaMenuInicial != 2);
    }

    // -------------------- FUNÇÃO DE MENU --------------------
    public static void MenuInicial(ref int escolhaMenuInicial)
    {
        do
        {
            Console.WriteLine("=== MENU INICIAL ===");
            Console.WriteLine("0. Cadastro de usuário");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Sair");
            Console.Write("Escolha uma opção: ");
            int.TryParse(Console.ReadLine(), out escolhaMenuInicial);
        } while (escolhaMenuInicial < 0 || escolhaMenuInicial > 2);
    }

    // -------------------- CADASTRO --------------------
    public static void CadastrarUsuario(string conexao)
    {
        using (MySqlConnection coon = new MySqlConnection(conexao))
        {
            coon.Open();

            Console.Write("Digite seu CPF: ");
            string CPF = Console.ReadLine();

            Console.Write("Digite seu nome: ");
            string nome = Console.ReadLine();

            Console.Write("Escreva sua data de nascimento (AAAA-MM-DD): ");
            string dataNascimento = Console.ReadLine();

            Console.Write("Digite seu e-mail: ");
            string gmail = Console.ReadLine();

            Console.Write("Crie uma senha forte: ");
            string senha = Console.ReadLine();

            Console.Write("Informe seu saldo inicial: ");
            double saldoInicial = double.Parse(Console.ReadLine());

            string sql = @"INSERT INTO clientes (CPF, nome, Data_Nascimento, gmail, senha, Saldo_atual)
                           VALUES (@CPF, @nome, @Data_Nascimento, @gmail, @senha, @Saldo_atual)";

            using (MySqlCommand comando = new MySqlCommand(sql, coon))
            {
                comando.Parameters.AddWithValue("@CPF", CPF);
                comando.Parameters.AddWithValue("@nome", nome);
                comando.Parameters.AddWithValue("@Data_Nascimento", dataNascimento);
                comando.Parameters.AddWithValue("@gmail", gmail);
                comando.Parameters.AddWithValue("@senha", senha);
                comando.Parameters.AddWithValue("@Saldo_atual", saldoInicial);
                comando.ExecuteNonQuery();
            }

            Console.WriteLine("\nUsuário cadastrado com sucesso!");
            Console.ReadKey();
        }
    }

    // -------------------- LOGIN --------------------
    public static void FazerLogin(string conexao)
    {
        Console.Write("Digite seu CPF: ");
        string CPF = Console.ReadLine();

        Console.Write("Digite sua senha: ");
        string senha = Console.ReadLine();

        using (MySqlConnection coon = new MySqlConnection(conexao))
        {
            coon.Open();
            string query = "SELECT CPF, nome, Saldo_atual FROM clientes WHERE CPF = @CPF AND senha = @senha";

            using (MySqlCommand comando = new MySqlCommand(query, coon))
            {
                comando.Parameters.AddWithValue("@CPF", CPF);
                comando.Parameters.AddWithValue("@senha", senha);

                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string nome = reader["nome"].ToString();
                        double saldoAtual = Convert.ToDouble(reader["Saldo_atual"]);

                        Console.Clear();
                        Console.WriteLine($"Bem-vindo, {nome}!");
                        MenuConta(conexao, CPF, saldoAtual);
                        return;
                    }
                }
            }

            Console.WriteLine("CPF ou senha incorretos!");
            Console.ReadKey();
        }
    }

    // -------------------- MENU CONTA --------------------
    public static void MenuConta(string conexao, string CPF, double saldoAtual)
    {
        int opcao = 0;

        do
        {
            Console.Clear();
            Console.WriteLine("=== MENU DA CONTA ===");
            Console.WriteLine("1. Consultar saldo");
            Console.WriteLine("2. Fazer depósito");
            Console.WriteLine("3. Fazer saque");
            Console.WriteLine("4. Fazer transferência");
            Console.WriteLine("5. Logout");
            Console.Write("Escolha uma opção: ");
            int.TryParse(Console.ReadLine(), out opcao);

            switch (opcao)
            {
                case 1:
                    Console.WriteLine($"Seu saldo atual é: R$ {saldoAtual:F2}");
                    break;

                case 2:
                    Console.Write("Valor do depósito: ");
                    double deposito = double.Parse(Console.ReadLine());
                    saldoAtual += deposito;
                    AtualizarSaldo(conexao, CPF, saldoAtual);
                    Console.WriteLine($"Depósito realizado! Novo saldo: R$ {saldoAtual:F2}");
                    break;

                case 3:
                    Console.Write("Valor do saque: ");
                    double saque = double.Parse(Console.ReadLine());
                    if (saque <= saldoAtual)
                    {
                        saldoAtual -= saque;
                        AtualizarSaldo(conexao, CPF, saldoAtual);
                        Console.WriteLine($"Saque realizado! Novo saldo: R$ {saldoAtual:F2}");
                    }
                    else
                    {
                        Console.WriteLine("Saldo insuficiente!");
                    }
                    break;

                case 4:
                    Console.Write("CPF do destinatário: ");
                    string cpfDestino = Console.ReadLine();
                    Console.Write("Valor da transferência: ");
                    double valorTransf = double.Parse(Console.ReadLine());

                    if (valorTransf <= saldoAtual)
                    {
                        if (Transferir(conexao, CPF, cpfDestino, valorTransf))
                        {
                            saldoAtual -= valorTransf;
                            AtualizarSaldo(conexao, CPF, saldoAtual);
                            Console.WriteLine("Transferência realizada com sucesso!");
                        }
                        else
                        {
                            Console.WriteLine("CPF do destinatário não encontrado!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Saldo insuficiente!");
                    }
                    break;

                case 5:
                    Console.WriteLine("Saindo da conta...");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();

        } while (opcao != 5);
    }

    // -------------------- ATUALIZAR SALDO --------------------
    public static void AtualizarSaldo(string conexao, string CPF, double novoSaldo)
    {
        using (MySqlConnection coon = new MySqlConnection(conexao))
        {
            coon.Open();
            string update = "UPDATE clientes SET Saldo_atual = @Saldo WHERE CPF = @CPF";
            using (MySqlCommand cmd = new MySqlCommand(update, coon))
            {
                cmd.Parameters.AddWithValue("@Saldo", novoSaldo);
                cmd.Parameters.AddWithValue("@CPF", CPF);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // -------------------- TRANSFERÊNCIA --------------------
    public static bool Transferir(string conexao, string cpfOrigem, string cpfDestino, double valor)
    {
        using (MySqlConnection coon = new MySqlConnection(conexao))
        {
            coon.Open();

            // Verifica se o destinatário existe
            string check = "SELECT COUNT(*) FROM clientes WHERE CPF = @CPF";
            using (MySqlCommand cmdCheck = new MySqlCommand(check, coon))
            {
                cmdCheck.Parameters.AddWithValue("@CPF", cpfDestino);
                long existe = (long)cmdCheck.ExecuteScalar();

                if (existe == 0)
                    return false;
            }

            // Atualiza saldo do destinatário
            string update = "UPDATE clientes SET Saldo_atual = Saldo_atual + @valor WHERE CPF = @CPF";
            using (MySqlCommand cmd = new MySqlCommand(update, coon))
            {
                cmd.Parameters.AddWithValue("@valor", valor);
                cmd.Parameters.AddWithValue("@CPF", cpfDestino);
                cmd.ExecuteNonQuery();
            }

            return true;
        }
    }
}