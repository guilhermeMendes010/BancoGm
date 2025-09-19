public class Program
{


    const int MAX = 20;
    static string[] nomes = new string[MAX];
    static string[] cpfs = new string[MAX];
    static string[] senhas = new string[MAX];
    static decimal[] saldos = new decimal[MAX];
    static int total = 0;

    static void Main()
    {
        string opcao;
        do
        {
            Console.Clear();
            Console.WriteLine("=== BANCO SIMPLES ===");
            Console.WriteLine("Data e hora: " + DateTime.Now);
            Console.WriteLine("1 - Cadastrar usuário");
            Console.WriteLine("2 - Login");
            Console.WriteLine("3 - Sair");
            Console.Write("Escolha: ");
            opcao = Console.ReadLine();

            if (opcao == "1") Cadastrar();
            
        }
        while (opcao != "3");

        Console.WriteLine("Fim do programa!");
    }
    static void Cadastrar()
    {
        if (total >= MAX)
        {
            Console.WriteLine("Limite atingido!");
            Console.ReadLine();
            return;
        }
        Console.Write(" nome: ");
        nomes[total] = Console.ReadLine();
        Console.Write(" CPF: ");
        cpfs[total] = Console.ReadLine();
        Console.Write("senha: ");
        senhas[total] = Console.ReadLine();
        Console.Write("saldo inicial: ");
        saldos[total] = decimal.Parse(Console.ReadLine());
    }

}