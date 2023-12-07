using System;
using System.Globalization;
namespace Questao1
{
    class ContaBancaria {
    public int NumeroConta { get; }
    public string TitularConta { get; set; }
    private double Saldo { get; set; }

    public ContaBancaria(int numeroConta, string titularConta, double depositoInicial = 0)
    {
        NumeroConta = numeroConta;
        TitularConta = titularConta;
        Saldo = depositoInicial;
    }
    public void Deposito(double valor)
    {
        Saldo += valor;
    }
    public void Saque(double valor)
    {
        if (Saldo >= valor)
        {
            Saldo -= valor;
        }
        else
        {
            Console.WriteLine("Saldo insuficiente para saque.");
        }
    }

    public override string ToString()
    {
        return $"Conta {NumeroConta}, Titular: {TitularConta}, Saldo: $ {Saldo:F2}";
    }
       
    }
}
