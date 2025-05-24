using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenAI_API; // Biblioteca de OpenAI
using OpenAI_API.Chat;
using Word = Microsoft.Office.Interop.Word;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Mi_chatGpt
{
    public partial class Form1 : Form
    {
        private string apiKey = "clave Aqui"; // Reemplaza con tu clave de OpenAI
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConn"].ConnectionString;
        private string respuestaApi = string.Empty;
        private OpenAIAPI openAiApi;

        public Form1()
        {
            InitializeComponent();
            openAiApi = new OpenAIAPI(apiKey); // Inicializa la API de OpenAI
        }

        // Botón Investigar
        private async void btnInvestigar_Click(object sender, EventArgs e)
        {
            string prompt = txtPrompt.Text;
            if (string.IsNullOrEmpty(prompt))
            {
                MessageBox.Show("Ingrese un prompt para investigar.");
                return;
            }

            lblEstado.Text = "Consultando API de ChatGPT...";
            respuestaApi = await ConsultarChatGptApi(prompt);
            txtRespuesta.Text = respuestaApi;
            lblEstado.Text = "Consulta completada. Revise el resultado.";

            // Guardar en base de datos
            GuardarEnBaseDatos(prompt, respuestaApi);

            // Estimar tokens (aproximado)
            int tokensEstimados = (prompt.Length + respuestaApi.Length) / 4;
            lblEstado.Text += $"\nTokens estimados: {tokensEstimados}";
        }

        // Botón Aprobar
        private void btnAprobar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(respuestaApi))
            {
                MessageBox.Show("No hay resultados para generar archivos.");
                return;
            }

            string carpeta = GenerarArchivos(txtPrompt.Text, respuestaApi);
            lblEstado.Text = $"Archivos generados en: {carpeta}";
        }

        // Botón Editar
        private void btnEditar_Click(object sender, EventArgs e)
        {
            respuestaApi = txtRespuesta.Text; // Actualiza la respuesta con los cambios
            lblEstado.Text = "Respuesta editada. Presione Aprobar para generar archivos.";
        }

        // Consultar API de ChatGPT
        private async Task<string> ConsultarChatGptApi(string prompt)
        {
            try
            {
                var chatRequest = openAiApi.Chat.CreateConversation();
                chatRequest.AppendUserInput(prompt);
                chatRequest.RequestParameters.Model = "gpt-3.5-turbo"; // Usa GPT-3.5-turbo para costos bajos
                chatRequest.RequestParameters.MaxTokens = 1000; // Límite de tokens en la respuesta
                chatRequest.RequestParameters.Temperature = 0.7; // Controla la creatividad (0 a 1)

                var response = await chatRequest.GetResponseFromChatbotAsync();
                return response;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar la API de ChatGPT: " + ex.Message);
                return string.Empty;
            }
        }

        // Guardar en SQL Server
        private void GuardarEnBaseDatos(string prompt, string respuesta)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Investigaciones (Prompt, Respuesta, Fecha) VALUES (@Prompt, @Respuesta, @Fecha)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Prompt", prompt);
                        cmd.Parameters.AddWithValue("@Respuesta", respuesta);
                        cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Datos guardados en la base de datos.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en la base de datos: " + ex.Message);
            }
        }

        // Generar Word y PowerPoint
        private string GenerarArchivos(string prompt, string respuesta)
        {
            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Investigacion_{DateTime.Now:yyyyMMdd_HHmmss}");
            Directory.CreateDirectory(carpeta);

            // Generar Word
            string wordPath = Path.Combine(carpeta, "Investigacion.docx");
            GenerarDocumentoWord(wordPath, prompt, respuesta);

            // Generar PowerPoint
            string pptPath = Path.Combine(carpeta, "Investigacion.pptx");
            GenerarPresentacionPowerPoint(pptPath, prompt, respuesta);

            return carpeta;
        }

        // Generar documento Word usando plantilla
        private void GenerarDocumentoWord(string path, string prompt, string respuesta)
        {
            Word.Application wordApp = new Word.Application();
            Word.Document doc;

            try
            {
                // Cargar plantilla (crear un .dotx con marcadores {Prompt} y {Respuesta})
                string plantillaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlantillaInvestigacion.dotx");
                doc = wordApp.Documents.Add(plantillaPath);

                // Reemplazar marcadores
                doc.Content.Find.Execute(FindText: "{Prompt}", ReplaceWith: prompt, Replace: Word.WdReplace.wdReplaceAll);
                doc.Content.Find.Execute(FindText: "{Respuesta}", ReplaceWith: respuesta, Replace: Word.WdReplace.wdReplaceAll);

                doc.SaveAs2(path);
                doc.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar Word: " + ex.Message);
            }
            finally
            {
                wordApp.Quit();
            }
        }

        // Generar presentación PowerPoint
        private void GenerarPresentacionPowerPoint(string path, string prompt, string respuesta)
        {
            PowerPoint.Application pptApp = new PowerPoint.Application();
            PowerPoint.Presentation ppt = pptApp.Presentations.Add();

            try
            {
                // Diapositiva 1: Título
                var slide1 = ppt.Slides.Add(1, PowerPoint.PpSlideLayout.ppLayoutText);
                slide1.Shapes.Title.TextFrame.TextRange.Text = "Investigación: " + prompt;

                // Diapositiva 2: Contenido
                var slide2 = ppt.Slides.Add(2, PowerPoint.PpSlideLayout.ppLayoutText);
                slide2.Shapes.Title.TextFrame.TextRange.Text = "Resultados";
                slide2.Shapes[2].TextFrame.TextRange.Text = respuesta;

                ppt.SaveAs(path);
                ppt.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar PowerPoint: " + ex.Message);
            }
            finally
            {
                pptApp.Quit();
            }
        }
    }
}

