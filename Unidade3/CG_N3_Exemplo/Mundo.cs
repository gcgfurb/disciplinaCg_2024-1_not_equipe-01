#define CG_DEBUG
#define CG_Gizmo      
#define CG_OpenGL      
// #define CG_OpenTK
// #define CG_DirectX      
#define CG_Privado  

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;

    private char rotuloAtual = '?';
    private Objeto objetoSelecionado = null;

    private readonly float[] _sruEixos =
    [
       -0.5f,  0.0f,   0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f,  -0.5f,   0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,   0.0f,  -0.5f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    ];

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      Utilitario.Diretivas();
#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      #region Cores
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      #endregion

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion

      List<Ponto4D> pontosAux = new List<Ponto4D>();
      objetoSelecionado = new Poligono(mundo, ref rotuloAtual, pontosAux);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      mundo.Desenhar(new Transformacao4D());

#if CG_Gizmo      
      Gizmo_Sru3D();
      Gizmo_BBox();
#endif
      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyPressed(Keys.L) && objetoSelecionado != null) // Auxiliar para verificar onde estão desenhadas as BBox dos polígonos 
      {
        Objeto objetoAtual = mundo.GrafocenaBuscaPoligonosMundo('@');

        while (objetoAtual != null && objetoAtual.PontosListaTamanho != 0)
        {
          objetoAtual.Bbox().Desenhar(new Transformacao4D());
          if (objetoAtual.objetosLista.Count > 0)
            objetoAtual = objetoAtual.GrafocenaBuscaPoligonosMundo(Utilitario.CharProximo(objetoAtual.rotulo));
        }
        objetoSelecionado.Bbox().Desenhar(new Transformacao4D());
        SwapBuffers();

        var BotarBreakpointAqui = 1; // Necessário botar breakpoint aqui para ver as BBox desenhadas
      }
      if (estadoTeclado.IsKeyPressed(Keys.Enter) && objetoSelecionado != null) // Desenha BBox e cria novo objeto
      {
        var transformada = new Transformacao4D();
        objetoSelecionado.Bbox().Desenhar(transformada);
        SwapBuffers();

        List<Ponto4D> pontosPoligonoTriangulo = new List<Ponto4D>();
        objetoSelecionado = new Poligono(objetoSelecionado, ref rotuloAtual, pontosPoligonoTriangulo);
      }

      if (estadoTeclado.IsKeyDown(Keys.Escape))
        Close();
      if (estadoTeclado.IsKeyPressed(Keys.Space))
      {
        if (objetoSelecionado == null)
          objetoSelecionado = mundo;
        objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
      }
      if (estadoTeclado.IsKeyPressed(Keys.G))                 //TODO: testar com grafo maior ,, irmãos
        mundo.GrafocenaImprimir("");
      if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null)
        Console.WriteLine(objetoSelecionado.ToString());
      if (estadoTeclado.IsKeyPressed(Keys.M) && objetoSelecionado != null)
        objetoSelecionado.MatrizImprimir();
      //TODO: não está atualizando a BBox com as transformações geométricas
      if (estadoTeclado.IsKeyPressed(Keys.I) && objetoSelecionado != null)
        objetoSelecionado.MatrizAtribuirIdentidade();
      if (estadoTeclado.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
      if (estadoTeclado.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
      if (estadoTeclado.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
      if (estadoTeclado.IsKeyPressed(Keys.Home) && objetoSelecionado != null)   //FIXME: problema depois de usa escala pto qquer, pois escala pto fixo não usa o novo centro da BBOX
        objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
      if (estadoTeclado.IsKeyPressed(Keys.End) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);
      if (estadoTeclado.IsKeyPressed(Keys.D1) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(10);
      if (estadoTeclado.IsKeyPressed(Keys.D2) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(-10);
      if (estadoTeclado.IsKeyPressed(Keys.D3) && objetoSelecionado != null)   //FIXME: problema depois de usa rotação pto qquer, não usa o novo centro da BBOX
        objetoSelecionado.MatrizRotacaoZBBox(10);
      if (estadoTeclado.IsKeyPressed(Keys.D4) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacaoZBBox(-10);
    
      if (estadoTeclado.IsKeyPressed(Keys.D) && objetoSelecionado != null) // Remover polígono
      {
        mundo.FilhoRemover(objetoSelecionado);
        objetoSelecionado = mundo;
      }
      if (estadoTeclado.IsKeyPressed(Keys.E) && objetoSelecionado != null) // Remover vértice do polígono
      {
        int janelaLargura = ClientSize.X;
        int janelaAltura = ClientSize.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPontoMouse = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        double menorDistancia = Double.MaxValue;
        int posicao = -1;

        for (int idx = 0; idx < objetoSelecionado.PontosListaTamanho; ++idx)
        {
          var distanciaAtual = Matematica.Distancia(sruPontoMouse, objetoSelecionado.pontosLista[idx]);

          if (menorDistancia > distanciaAtual)
          {
            menorDistancia = distanciaAtual;
            posicao = idx;
          }
        }
        if (posicao != -1)
          objetoSelecionado.PontosRemover(posicao);
      }
      if (estadoTeclado.IsKeyDown(Keys.V) && objetoSelecionado != null) // Mover vértice do polígono
      {
        int janelaLargura = ClientSize.X;
        int janelaAltura = ClientSize.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPontoMouse = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        double menorDistancia = Double.MaxValue;
        int posicao = -1;

        for (int idx = 0; idx < objetoSelecionado.PontosListaTamanho; ++idx)
        {
          var distanciaAtual = Matematica.Distancia(sruPontoMouse, objetoSelecionado.pontosLista[idx]);

          if (menorDistancia > distanciaAtual)
          {
            menorDistancia = distanciaAtual;
            posicao = idx;
          }
        }

        if (posicao != -1)
          objetoSelecionado.PontosAlterar(sruPontoMouse, posicao);
      }
      if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null) // Mudar tipo polígono
      {
        if (objetoSelecionado.PrimitivaTipo == PrimitiveType.LineLoop)
          objetoSelecionado.PrimitivaTipo = PrimitiveType.LineStrip;
        else
          objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
      }
      if (estadoTeclado.IsKeyPressed(Keys.R) && objetoSelecionado != null) // Mudar cor para Vermelho
      {
        objetoSelecionado.ShaderObjeto = new("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      }
      if (estadoTeclado.IsKeyPressed(Keys.G) && objetoSelecionado != null) // Mudar cor para Verde
      {
        objetoSelecionado.ShaderObjeto = new("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      }
      if (estadoTeclado.IsKeyPressed(Keys.B) && objetoSelecionado != null) // Mudar cor para Azul
      {
        objetoSelecionado.ShaderObjeto = new("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      }

      #endregion

      #region  Mouse

      if (MouseState.IsButtonPressed(MouseButton.Left)) // Seleciona polígono, verifica se clique está dentro da BBox com ScanLine e desenha
      {
        int janelaLargura = ClientSize.X;
        int janelaAltura = ClientSize.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        Objeto objetoAtual = mundo.GrafocenaBuscaPoligonosMundo('@');
        bool dentroPoligono = false;

        while (objetoAtual != null && objetoAtual.PontosListaTamanho != 0)
        {
          if (objetoAtual.Bbox().Dentro(sruPonto))
          {
            int contadorDireita = 0;

            for (int idx = 0; idx < objetoAtual.PontosListaTamanho - 1; ++idx)
            {
              if (Matematica.ScanLine(sruPonto, objetoAtual.pontosLista[idx], objetoAtual.pontosLista[idx + 1]))
                contadorDireita += 1;
            }

            if (Matematica.ScanLine(sruPonto, objetoAtual.pontosLista[objetoAtual.PontosListaTamanho - 1], objetoAtual.pontosLista[0]))
              contadorDireita += 1;

            if (contadorDireita % 2 != 0)
            {
              objetoSelecionado = objetoAtual;
              dentroPoligono = true;
              break;
            }
          }

          if (objetoAtual.objetosLista.Count > 0)
            objetoAtual = objetoAtual.GrafocenaBuscaPoligonosMundo(Utilitario.CharProximo(objetoAtual.rotulo));
          else
            objetoAtual = mundo.GrafocenaBuscaPoligonosMundo(Utilitario.CharProximo(objetoAtual.rotulo));
          if (objetoAtual == null)
          {
            break;
          }
        }

        if (dentroPoligono)
        {
          Transformacao4D transformacaoAux = new Transformacao4D();
          objetoSelecionado.Bbox().Desenhar(transformacaoAux);
          SwapBuffers();
        }

      }
      if (MouseState.IsButtonPressed(MouseButton.Right))
      {
        int janelaLargura = ClientSize.X;
        int janelaAltura = ClientSize.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        objetoSelecionado.PontosAdicionar(sruPonto);
      }
      if (MouseState.IsButtonDown(MouseButton.Right) && objetoSelecionado != null)
      {
        int janelaLargura = ClientSize.X;
        int janelaAltura = ClientSize.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        if (objetoSelecionado.PontosListaTamanho != 1)
          objetoSelecionado.PontosAlterar(sruPonto, 0);
        else
          objetoSelecionado.PontosAdicionar(sruPonto);
      }
      #endregion
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
      GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Gizmo_Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      var transform = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("transform", transform);
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("transform", transform);
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("transform", transform);
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

#if CG_Gizmo
    private void Gizmo_BBox()   // Mantém desenho da BBox na tela
    {
      if (objetoSelecionado != null && objetoSelecionado != mundo && objetoSelecionado.objetosLista.Count != 0)
      {
        var transformada = new Transformacao4D();
        objetoSelecionado.Bbox().Desenhar(transformada);

        return;
      }
    }
#endif    

  }
}
