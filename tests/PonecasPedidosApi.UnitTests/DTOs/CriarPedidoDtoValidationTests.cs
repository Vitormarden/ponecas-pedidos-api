using System.ComponentModel.DataAnnotations;
using PonecasPedidosApi.DTOs;
using Xunit;

namespace PonecasPedidosApi.UnitTests.DTOs;

public class CriarPedidoDtoValidationTests
{
    // Atributos de record ficam nos parametros do construtor primario (como o MVC espera),
    // entao validamos cada parametro com os atributos dele.
    private static List<ValidationResult> Validar(object obj)
    {
        var results = new List<ValidationResult>();
        var type = obj.GetType();
        var ctor = type.GetConstructors().Single();

        foreach (var parametro in ctor.GetParameters())
        {
            var atributos = parametro.GetCustomAttributes(typeof(ValidationAttribute), inherit: true)
                .Cast<ValidationAttribute>();
            var valor = type.GetProperty(parametro.Name!)!.GetValue(obj);
            var contexto = new ValidationContext(obj) { MemberName = parametro.Name };
            Validator.TryValidateValue(valor!, contexto, results, atributos);
        }

        return results;
    }

    [Fact]
    public void ItemValido_NaoGeraErros()
    {
        var erros = Validar(new CriarItemPedidoDto("Caneca", 2, 39.90m));

        Assert.Empty(erros);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NomeProdutoVazioOuEmBranco_GeraErro(string nome)
    {
        var erros = Validar(new CriarItemPedidoDto(nome, 1, 1.00m));

        Assert.NotEmpty(erros);
    }

    [Fact]
    public void NomeProdutoMaiorQue200_GeraErro()
    {
        var erros = Validar(new CriarItemPedidoDto(new string('a', 201), 1, 1.00m));

        Assert.NotEmpty(erros);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10001)]
    public void QuantidadeForaDoIntervalo_GeraErro(int quantidade)
    {
        var erros = Validar(new CriarItemPedidoDto("Caneca", quantidade, 1.00m));

        Assert.NotEmpty(erros);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10000)]
    public void QuantidadeNosLimites_NaoGeraErro(int quantidade)
    {
        var erros = Validar(new CriarItemPedidoDto("Caneca", quantidade, 1.00m));

        Assert.Empty(erros);
    }

    [Theory]
    [InlineData("0.00")]
    [InlineData("0.001")]
    [InlineData("-1.00")]
    [InlineData("1000000")]
    public void PrecoForaDoIntervalo_GeraErro(string preco)
    {
        var erros = Validar(new CriarItemPedidoDto("Caneca", 1, decimal.Parse(preco, System.Globalization.CultureInfo.InvariantCulture)));

        Assert.NotEmpty(erros);
    }

    [Fact]
    public void PedidoSemItens_GeraErro()
    {
        var erros = Validar(new CriarPedidoDto(new List<CriarItemPedidoDto>()));

        Assert.NotEmpty(erros);
    }

    [Fact]
    public void PedidoComItens_NaoGeraErro()
    {
        var erros = Validar(new CriarPedidoDto(new List<CriarItemPedidoDto> { new("Caneca", 1, 1.00m) }));

        Assert.Empty(erros);
    }
}
