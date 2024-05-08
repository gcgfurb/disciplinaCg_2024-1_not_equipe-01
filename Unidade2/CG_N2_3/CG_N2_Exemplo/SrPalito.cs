using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Reflection;

namespace gcgcg
{
  internal class SrPalito : Objeto
  {
    Ponto4D pontoPe;
    Ponto4D pontoCabeca;
    double inclinacao;
    double raio;
    int restante;

    bool inverter = false;

    public SrPalito(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, new Ponto4D(0, 0), new Ponto4D(0.5, 0.5))
    {

    }

    public SrPalito(Objeto _paiRef, ref char _rotulo, Ponto4D ptoIni, Ponto4D ptoFim) : base(_paiRef, ref _rotulo)
    {
      pontoPe = ptoIni;
      pontoCabeca = ptoFim;
      raio = Matematica.distancia(pontoPe, pontoCabeca);
      restante = 5;
      inclinacao = 9;

      PrimitivaTipo = PrimitiveType.Lines;
      PrimitivaTamanho = 0.5f;

      PontosAdicionar(ptoIni);
      PontosAdicionar(ptoFim);
      Atualizar();
    }

    public void AtualizarPe(double peInc)
    {
      pontoPe.X += peInc;
      pontoCabeca.X += peInc;


      PontosAlterar(pontoPe, 0);
      PontosAlterar(pontoCabeca, 1);
    }

    public void AtualizarRaio(double raioInc)
    {

      double x1 = 0;
      double y1 = 0;
      // * 5 pois 360 / 5 = 72. E para pegar em graus
      double inclinacaoEmGraus = inclinacao * 5;
      double raio = Matematica.distancia(pontoPe, pontoCabeca);
      double novoRaio;
      inverter = false;
      if (restante <= 0)
      {
        if (restante == 0)
        {
          raio = 0.1;
          if (raioInc > 0)
          {
            restante += 1;
          }
          else
          {
            restante -= 1;
          }

          novoRaio = raio / restante;
        }
        else
        {
          inverter = true;
          if (raioInc > 0)
          {
            novoRaio = raio + (raio / restante);
            restante += 1;
          }
          else
          {
            novoRaio = raio - (raio / restante);
            restante -= 1;
          }
        }
      }
      else
      {
        if (raioInc > 0)
        {
          novoRaio = raio + (raio / restante);
          restante += 1;
        }
        else
        {
          novoRaio = raio - (raio / restante);
          restante -= 1;
        }

      }

      double inclinacaoEmRadianos = inclinacaoEmGraus * Math.PI / 180;

      // Calcula o ponto resultante
      double x2 = x1 + novoRaio * Math.Cos(inclinacaoEmRadianos);
      double y2 = y1 + novoRaio * Math.Sin(inclinacaoEmRadianos);

      if (inverter)
      {
        x2 *= -1;
        y2 *= -1;
      }

      pontoCabeca.X = x2 + pontoPe.X;
      pontoCabeca.Y = y2 + pontoPe.Y;
      PontosAlterar(pontoCabeca, 1);
    }

    public void AtualizarAngulo(double anguloInc)
    {

      double x1 = 0;
      double y1 = 0;
      // * 5 pois 360 / 5 = 72. E para pegar em graus
      inclinacao += anguloInc;
      double inclinacaoEmGraus = inclinacao * 5;
      double raio = Matematica.distancia(pontoPe, pontoCabeca);

      double inclinacaoEmRadianos = inclinacaoEmGraus * Math.PI / 180;

      // Calcula o ponto resultante
      double x2 = x1 + raio * Math.Cos(inclinacaoEmRadianos);
      double y2 = y1 + raio * Math.Sin(inclinacaoEmRadianos);

      if (inverter)
      {
        x2 *= -1;
        y2 *= -1;
      }

      pontoCabeca.X = x2 + pontoPe.X;
      pontoCabeca.Y = y2 + pontoPe.Y;

      PontosAlterar(pontoCabeca, 1);
    }

    private void Atualizar()
    {
      ObjetoAtualizar();
    }
  }
}