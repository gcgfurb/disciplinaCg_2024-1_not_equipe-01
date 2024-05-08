#define CG_Debug

using System;
using System.Collections.Generic;
using System.Threading;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Spline : Objeto
  {
    int qtdPontos = 10;

    public Spline(Objeto _paiRef, ref char _rotulo, List<Ponto4D> lista) : base(_paiRef, ref _rotulo) {
        PrimitivaTipo = PrimitiveType.Lines;
        PrimitivaTamanho = 20;

        AtualizarSpline(lista);
    }

    public void SplineQtdPonto(int qtdPontos)
    {
      this.qtdPontos += qtdPontos;

      if (this.qtdPontos < 1)
        this.qtdPontos = 1;
      else if (this.qtdPontos > 10)
        this.qtdPontos = 10;
    }

    public void Atualizar()
    {



        base.ObjetoAtualizar();
    }

    public void AtualizarSpline(List<Ponto4D> pontos)
    {  
        List<Ponto4D> lista = new List<Ponto4D>(0);

        double aux = 1.0 / qtdPontos; 
        aux = Math.Truncate(10000 * aux) / 10000;

        for (double incremento = 0; incremento <= 1.0;)
        {
            Ponto4D AB = new Ponto4D();
            AB.X = pontos[0].X + (pontos[1].X - pontos[0].X) * incremento;
            AB.Y = pontos[0].Y + (pontos[1].Y - pontos[0].Y) * incremento;

            Ponto4D BC = new Ponto4D();
            BC.X = pontos[1].X + (pontos[2].X - pontos[1].X) * incremento;
            BC.Y = pontos[1].Y + (pontos[2].Y - pontos[1].Y) * incremento;

            Ponto4D CD = new Ponto4D();
            CD.X = pontos[2].X + (pontos[3].X - pontos[2].X) * incremento;
            CD.Y = pontos[2].Y + (pontos[3].Y - pontos[2].Y) * incremento;

            Ponto4D ABBC = new Ponto4D();
            ABBC.X = AB.X + (BC.X - AB.X) * incremento;
            ABBC.Y = AB.Y + (BC.Y - AB.Y) * incremento;

            Ponto4D BCCD = new Ponto4D();
            BCCD.X = BC.X + (CD.X - BC.X) * incremento;
            BCCD.Y = BC.Y + (CD.Y - BC.Y) * incremento;

            Ponto4D ABBCBCCD = new Ponto4D();
            ABBCBCCD.X = ABBC.X + (BCCD.X - ABBC.X) * incremento;
            ABBCBCCD.Y = ABBC.Y + (BCCD.Y - ABBC.Y) * incremento;

            lista.Add(ABBCBCCD);

            incremento += aux;
            incremento = Math.Truncate(10000 * incremento) / 10000;
        }
        
        this.PrimitivaTipo = PrimitiveType.LineStrip;
        this.PrimitivaTamanho = 1;
        SetPonto4Ds(lista);

        base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Spline _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
