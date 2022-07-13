using SingletonTest.Logging;
using SingletonTest.Models;
using System.Text.Json;

// CABECALHO
// Fiz uma classe simples para apenas loggar os erros no PC mesmo. Fiz algo parecido que faco nos meus projetos que tem AppCenter, em Xamarin.
// Coloquei apenas duas excecoes como exemplo e um aviso, para quando queremos rastrear algo.
// Separei os logs por dias, entao por classes chamados (classes que estouraram a excecao) e por fim os arquivos por "HoraMinSegMil_TipoLog_MetodoChamador.txt";

var menuOptions = new Dictionary<char, Func<Task>>()
{
    { '1', ThrowExceptionsAndWarnings }
};

var canContinue = false;

do
{
    Console.WriteLine("1 - Gerar exceções e avisos.\n");
    Console.Write("Entrada: ");

    var input = HandleReadKey(Console.ReadKey());

    canContinue = await HandleMenuOption(input, menuOptions);

} while (!canContinue);

DataLogging.CurrentInstance.OpenLogsFolder();

Console.Clear();
Console.WriteLine("Pressione qualquer tecla para finalizar... (Menos a de desligar xD)");
Console.ReadKey();

char HandleReadKey(ConsoleKeyInfo keyDown) =>
    keyDown.KeyChar;

async Task<bool> HandleMenuOption(char pressKey, Dictionary<char, Func<Task>> optionsAvaliable)
{
    if (optionsAvaliable.Keys.Contains(pressKey))
    {
        await optionsAvaliable[pressKey].Invoke();
        return true;
    }

    Console.Clear();

    Console.WriteLine("Opção inválida!");
    await Task.Delay(4000);

    Console.Clear();

    return false;
}

async Task ThrowExceptionsAndWarnings()
{
    var dataLoggin = DataLogging.CurrentInstance;

    var operationResult = WarningGenerator1();
    if (!operationResult)
        await dataLoggin.LogCustomMessage("The input E-mail already exists!",
            nameof(Program),
            nameof(ThrowExceptionsAndWarnings),
            usedParameters: new()
            {
                { "Name", "Wesley Kuhn" },
                { "City", "São Pedro" },
                { "E-mail", "wesley@company.com" }
            });

    try
    {
        ExceptionGenerator1();
    }
    catch (Exception ex1)
    {
        await dataLoggin.LogException(ex1, nameof(Program), nameof(ThrowExceptionsAndWarnings), nameof(ExceptionGenerator1));
    }

    try
    {
        ExceptionGenerator2();
    }
    catch (Exception ex2)
    {
        await dataLoggin.LogException(ex2, nameof(Program), nameof(ThrowExceptionsAndWarnings), nameof(ExceptionGenerator2));
    }

    try
    {
        ExceptionGenerator3();
    }
    catch (Exception ex2)
    {
        await dataLoggin.LogException(ex2, nameof(Program), nameof(ThrowExceptionsAndWarnings), nameof(ExceptionGenerator2));
    }
}

void ExceptionGenerator1()
{
    _ = File.Open("kkkkkkkkkkkkkkk", FileMode.Open);
}

void ExceptionGenerator2()
{
    _ = JsonSerializer.Deserialize<MyObject>("serializacaoTodaBugada");
}

void ExceptionGenerator3()
{
    _ = int.Parse("we5ley");
}

bool WarningGenerator1()
{
    byte one = 1;
    byte two = 2;

    return one == two;
}