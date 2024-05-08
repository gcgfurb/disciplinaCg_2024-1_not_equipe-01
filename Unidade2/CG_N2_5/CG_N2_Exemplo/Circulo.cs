using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Circulo : Objeto
  {
    public double raio = 0.0;
    public Circulo(Objeto _paiRef, ref char _rotulo, double _raio, Ponto4D ptoDeslocamento) : base(_paiRef, ref _rotulo) 
    {
        raio = _raio;

        PrimitivaTipo = PrimitiveType.LineLoop;
        PrimitivaTamanho = 5;

        for (int i = 0; i < 72; i++)
        {
            double angulo = 360.0 / 72.0 * i;
            Ponto4D ponto = Matematica.GerarPtosCirculo(angulo, _raio);
            ponto.X += ptoDeslocamento.X;
            ponto.Y += ptoDeslocamento.Y;

            pontosLista.Add(ponto);
        }

        Atualizar();
    }

    public void Atualizar()
    {
      base.ObjetoAtualizar();
    }

    public void AtualizarDeslocamento(Ponto4D ptoDeslocamento)
    {
      LimpaPontos4D();

      for (int i = 0; i < 72; i++)
        {
            double angulo = 360.0 / 72.0 * i;
            Ponto4D ponto = Matematica.GerarPtosCirculo(angulo, raio);
            ponto.X += ptoDeslocamento.X;
            ponto.Y += ptoDeslocamento.Y;

            pontosLista.Add(ponto);
        }

        base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Círculo\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif
  }
}