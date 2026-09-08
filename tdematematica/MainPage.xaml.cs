using Microsoft.Maui.Controls;

namespace tdematematica
{
    public partial class MainPage : ContentPage
    {
        int numero1Valor = 0;
        int numero2Valor = 0;
        int operacaoValor = 0;

        int respostaCerta = 0;
        int quantidadeAcertos = 0;
        int quantidadeErros = 0;
        int totalPontos = 0;
        int questaoAtual = 1;
        int segundos = 30;
        int limite = 10;
        int pontosBase = 10;

        bool jaRespondeu = false;
        bool mudandoTexto = false;

        Random aleatorio = new Random();

        List<int> operacoes = new List<int>();

        public MainPage()
        {
            InitializeComponent();

            dificuldade.Items.Add("Fácil (1-10)");
            dificuldade.Items.Add("Médio (1-50)");
            dificuldade.Items.Add("Difícil (1-100)");

            dificuldade.SelectedIndex = 0;
        }

        private async void iniciarJogo(object sender, EventArgs e)
        {
            operacoes.Clear();

            if (adicao.IsChecked)
            {
                operacoes.Add(1);
            }

            if (subtracao.IsChecked)
            {
                operacoes.Add(2);
            }

            if (multiplicacao.IsChecked)
            {
                operacoes.Add(3);
            }

            if (divisao.IsChecked)
            {
                operacoes.Add(4);
            }

            if (operacoes.Count == 0)
            {
                await DisplayAlertAsync("ERRO",
                    "Escolha pelo menos uma operação!", "OK");

                return;
            }

            switch (dificuldade.SelectedIndex)
            {
                case 1:
                    limite = 50;
                    pontosBase = 20;
                    break;

                case 2:
                    limite = 100;
                    pontosBase = 30;
                    break;

                default:
                    limite = 10;
                    pontosBase = 10;
                    break;
            }

            questaoAtual = 1;
            quantidadeAcertos = 0;
            quantidadeErros = 0;
            totalPontos = 0;

            acertos.Text = "Certas: 0";
            erros.Text = "Erradas: 0";
            pontos.Text = "Pontos: 0";

            configuracao.IsVisible = false;
            jogo.IsVisible = true;

            GerarConta();
        }

        private async void conferirResposta(object sender, EventArgs e)
        {
            int respostaUsuario = 0;

            if (jaRespondeu)
            {
                return;
            }

            try
            {
                respostaUsuario = Convert.ToInt32(resposta.Text);
            }
            catch
            {
                await DisplayAlertAsync("ERRO",
                    "Digite um número válido!", "OK");

                return;
            }

            if (respostaUsuario == respostaCerta)
            {
                await VerificarResposta(true, false);
            }
            else
            {
                await VerificarResposta(false, false);
            }
        }

        public void GerarConta()
        {
            jaRespondeu = false;

            botaoResponder.IsEnabled = true;
            resposta.IsEnabled = true;

            resposta.Text = "";
            mensagem.Text = "";
            imagemResultado.Source = "question.png";

            numeroQuestao.Text = $"Pergunta {questaoAtual} de 10";

            progressoQuestoes.Progress =
                Convert.ToDouble(questaoAtual - 1) / 10;

            int posicao = aleatorio.Next(0, operacoes.Count);

            operacaoValor = operacoes[posicao];

            numero1Valor = aleatorio.Next(1, limite + 1);
            numero2Valor = aleatorio.Next(1, limite + 1);

            switch (operacaoValor)
            {
                case 1:
                    respostaCerta = numero1Valor + numero2Valor;
                    operacao.Text = "+";
                    break;

                case 2:
                    if (numero2Valor > numero1Valor)
                    {
                        int auxiliar = numero1Valor;

                        numero1Valor = numero2Valor;
                        numero2Valor = auxiliar;
                    }

                    respostaCerta = numero1Valor - numero2Valor;
                    operacao.Text = "-";
                    break;

                case 3:
                    respostaCerta = numero1Valor * numero2Valor;
                    operacao.Text = "*";
                    break;

                case 4:
                    numero2Valor = aleatorio.Next(1, limite + 1);
                    respostaCerta = aleatorio.Next(1, limite + 1);
                    numero1Valor = numero2Valor * respostaCerta;

                    operacao.Text = "÷";
                    break;
            }

            numero1.Text = Convert.ToString(numero1Valor);
            numero2.Text = Convert.ToString(numero2Valor);

            IniciarTempo();
        }

        private void IniciarTempo()
        {
            segundos = 30;

            tempo.Text = $"Tempo: {segundos} segundos";
            tempo.TextColor = Colors.Green;

            progressoTempo.Progress = 1;
            progressoTempo.ProgressColor = Colors.Green;

            Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (jaRespondeu)
                {
                    return false;
                }

                segundos--;

                tempo.Text = $"Tempo: {segundos} segundos";

                progressoTempo.Progress =
                    Convert.ToDouble(segundos) / 30;

                if (segundos <= 10 && segundos > 5)
                {
                    tempo.TextColor = Colors.Orange;
                    progressoTempo.ProgressColor = Colors.Orange;
                }

                if (segundos <= 5)
                {
                    tempo.TextColor = Colors.Red;
                    progressoTempo.ProgressColor = Colors.Red;
                }

                if (segundos <= 0)
                {
                    _ = VerificarResposta(false, true);

                    return false;
                }

                return true;
            });
        }

        private async Task VerificarResposta(bool acertou,
                                             bool tempoTerminou)
        {
            if (jaRespondeu)
            {
                return;
            }

            jaRespondeu = true;

            botaoResponder.IsEnabled = false;
            resposta.IsEnabled = false;

            if (acertou)
            {
                quantidadeAcertos++;

                totalPontos = totalPontos + pontosBase + segundos;

                acertos.Text = $"Certas: {quantidadeAcertos}";
                pontos.Text = $"Pontos: {totalPontos}";

                mensagem.Text = "Acertou!";
                mensagem.TextColor = Colors.Green;

                imagemResultado.Source = "win.png";
            }
            else
            {
                quantidadeErros++;

                erros.Text = $"Erradas: {quantidadeErros}";

                if (tempoTerminou)
                {
                    mensagem.Text =
                        $"Acabou o tempo! Resposta: {respostaCerta}";
                }
                else
                {
                    mensagem.Text =
                        $"Resposta errada! Resposta: {respostaCerta}";
                }

                mensagem.TextColor = Colors.Red;

                imagemResultado.Source = "loose.png";
            }

            await Task.Delay(1500);

            if (questaoAtual >= 10)
            {
                await FinalizarJogo();
            }
            else
            {
                questaoAtual++;

                GerarConta();
            }
        }

        private async Task FinalizarJogo()
        {
            string classificacao = "";

            jaRespondeu = true;
            progressoQuestoes.Progress = 1;

            if (quantidadeAcertos >= 9)
            {
                classificacao = "Excelente";
            }
            else if (quantidadeAcertos >= 7)
            {
                classificacao = "Muito bom";
            }
            else if (quantidadeAcertos >= 5)
            {
                classificacao = "Bom";
            }
            else
            {
                classificacao = "Continue praticando";
            }

            await DisplayAlertAsync("Resultado Final",
                $"Pontos: {totalPontos}\n" +
                $"Acertos: {quantidadeAcertos}\n" +
                $"Erros: {quantidadeErros}\n" +
                $"Classificação: {classificacao}",
                "OK");

            jogo.IsVisible = false;
            configuracao.IsVisible = true;
        }

        private void mudouResposta(object sender,
                                    TextChangedEventArgs e)
        {
            string texto = "";

            if (mudandoTexto)
            {
                return;
            }

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                return;
            }

            foreach (char letra in e.NewTextValue)
            {
                if (char.IsDigit(letra))
                {
                    texto = texto + letra;
                }
            }

            if (texto != e.NewTextValue)
            {
                mudandoTexto = true;

                resposta.Text = texto;

                mudandoTexto = false;
            }
        }
    }
}
