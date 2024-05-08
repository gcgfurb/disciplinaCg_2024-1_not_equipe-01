using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Circulo : Objeto
  {
    public Circulo(Objeto _paiRef, ref char _rotulo, double _raio, Ponto4D ptoDeslocamento) : base(_paiRef, ref _rotulo) 
    {
        PrimitivaTipo = PrimitiveType.Points;
        PrimitivaTamanho = 5;

        for (int i = 0; i < 72; i++)
        {
            double angulo = 360.0 / 72.0 * i;
            Ponto4D ponto = Matematica.GerarPtosCirculo(angulo, _raio);
            pontosLista.Add(ponto);
        }

        Atualizar();
    }

    public void Atualizar()
    {
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