using Academia.Classes;
using Microsoft.Data.Sqlite;

namespace Academia;

public partial class FrmExercicio : ContentPage
{
    Exercicios exer = new();

    List<Exercicios> listaexercicios { get; set; } = new List<Exercicios>();
    public FrmExercicio()
    {
        InitializeComponent();

      



        // Obtenha os exerc�cios do banco de dados ou qualquer outra fonte de dados
        listaexercicios = GetExerciciosFromDatabase();

        // Defina a origem de dados da ListView
        ExerciciosListView.ItemsSource = listaexercicios;
    }

    private void NovoExercicioClicked(object sender, EventArgs e)
    {
        if (NovoExercicioLayout.IsVisible == false)
        {
            NovoExercicioLayout.IsVisible = true;
        }
        else
        {
            NovoExercicioLayout.IsVisible = false;
        }

    }

    private void CadastrarExercicioClicked(object sender, EventArgs e)
    {
        // Verificar se os campos obrigat�rios foram preenchidos
        if (string.IsNullOrWhiteSpace(TxtExercicio.Text) || string.IsNullOrWhiteSpace(TxtSeries.Text) || string.IsNullOrWhiteSpace(TxtRepeticoes.Text))
        {
            // Mostrar uma mensagem de erro ou tomar a a��o apropriada
            // Por exemplo: exibir um alerta ao usu�rio informando que todos os campos devem ser preenchidos
            DisplayAlert("Erro", "Por favor, preencha todos os campos obrigat�rios.", "OK");
            return;
        }

        int series;
        if (!int.TryParse(TxtSeries.Text, out series))
        {
            // Mostrar uma mensagem de erro ou tomar a a��o apropriada
            // Por exemplo: exibir um alerta ao usu�rio informando que o campo de s�ries deve ser um n�mero
            DisplayAlert("Erro", "O campo de s�ries deve ser um n�mero v�lido.", "OK");
            return;
        }

        int repeticoes;
        if (!int.TryParse(TxtRepeticoes.Text, out repeticoes))
        {
            // Mostrar uma mensagem de erro ou tomar a a��o apropriada
            // Por exemplo: exibir um alerta ao usu�rio informando que o campo de repeti��es deve ser um n�mero
            DisplayAlert("Erro", "O campo de repeti��es deve ser um n�mero v�lido.", "OK");
            return;
        }

       

        string databaseFilename = "academia.db";
        string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseFilename);
        string connectionString = $"Data Source={databasePath}";


        using (SqliteConnection connection = new(connectionString))
        {
            connection.Open();

            // Crie uma tabela "exercicio" se ainda n�o existir
            string createTableQuery = "CREATE TABLE IF NOT EXISTS exercicio (id_exercicio INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT, nseries INTEGER, repeticao INTEGER, peso INTEGER, descricao TEXT)";
            using (SqliteCommand command = new(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            // Aqui voc� pode realizar a l�gica para inserir um novo exerc�cio
            Exercicios exer = new Exercicios();
            exer.Nome = TxtExercicio.Text;
            exer.Nseries = series;
            exer.Repeticao = repeticoes;
            if (!string.IsNullOrWhiteSpace(TxtPeso.Text))
            {
                exer.Peso = Convert.ToInt32(TxtPeso.Text);
            }
            exer.Descricao = TxtDescricao.Text;

            string insertQuery = $"INSERT INTO exercicio (nome, nseries, repeticao, peso, descricao) VALUES ('{exer.Nome}', {exer.Nseries}, {exer.Repeticao}, {exer.Peso}, '{exer.Descricao}')";
            using (SqliteCommand insertCommand = new(insertQuery, connection))
            {
                insertCommand.ExecuteNonQuery();
            }

            connection.Close();
        }

        //testess
        TxtExercicio.Text = string.Empty;
        TxtSeries.Text = string.Empty;
        TxtRepeticoes.Text = string.Empty;
        TxtPeso.Text = string.Empty;
        TxtDescricao.Text = string.Empty;

        // Obtenha os exerc�cios do banco de dados ou qualquer outra fonte de dados
        List<Exercicios> listaexercicios = GetExerciciosFromDatabase();

        // Defina a origem de dados da ListView
        ExerciciosListView.ItemsSource = listaexercicios;
    }



    private List<Exercicios> GetExerciciosFromDatabase()
    {
        List<Exercicios> exercicios = new List<Exercicios>();

        string databaseFilename = "academia.db";
        string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseFilename);
        string connectionString = $"Data Source={databasePath}";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM exercicio";

            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string nome = reader.GetString(1);
                        int nseries = reader.GetInt32(2);
                        int repeticao = reader.GetInt32(3);
                        int peso = reader.GetInt32(4);
                        string descricao = reader.GetString(5);

                        Exercicios exercicio = new Exercicios(id, nome, nseries,repeticao, peso, descricao);
                        exercicios.Add(exercicio);
                    }
                }
            }

            connection.Close();
        }

        return exercicios;
    }

    private List<Exercicios> GetExerciciosLikeFromDatabase(string searchText)
    {
        string databaseFilename = "academia.db";
        string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseFilename);
        string connectionString = $"Data Source={databasePath}";

        List<Exercicios> exercicios = new();

        using (SqliteConnection connection = new(connectionString))
        {
            connection.Open();

            // Execute a consulta utilizando o filtro LIKE
            string query = $"SELECT * FROM exercicio WHERE nome LIKE '{searchText}%'";
            using (SqliteCommand command = new(query, connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Leia os dados do leitor e crie objetos Exercicio
                    int id = reader.GetInt32(0);
                    string nome = reader.GetString(1);
                    int nseries = reader.GetInt32(2);
                    int repeticao = reader.GetInt32(3);
                    int peso = reader.GetInt32(4);
                    string descricao = reader.GetString(5);

                    Exercicios exercicio = new Exercicios(id, nome, nseries, repeticao, peso, descricao);
                    exercicios.Add(exercicio);
                }
            }

            connection.Close();
        }

        return exercicios;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue;

        // Realize a l�gica de consulta no banco de dados com o filtro LIKE
        List<Exercicios> searchResults = GetExerciciosLikeFromDatabase(searchText);

        // Atualize a fonte de dados da ListView com os resultados da pesquisa
        ExerciciosListView.ItemsSource = searchResults;
    }

    private void ExercicioTapped(object sender, EventArgs e)
    {
        // Obt�m o exerc�cio selecionado
        var exercicio = (Exercicios)((Label)sender).BindingContext;

        // Verifica o estado atual dos detalhes
        bool detalhesVisiveis = DetalhesStackLayout.IsVisible;

        // Verifica se o exerc�cio selecionado � o mesmo que est� sendo exibido atualmente nos detalhes
        bool mesmoExercicio = DetalhesStackLayout.BindingContext == exercicio;

        // Se os detalhes est�o vis�veis e o mesmo exerc�cio foi clicado novamente, fecha os detalhes
        if (detalhesVisiveis && mesmoExercicio)
        {
            DetalhesStackLayout.IsVisible = false;
        }
        else
        {
            // Define o contexto de dados do DetalhesStackLayout com o exerc�cio selecionado
            DetalhesStackLayout.BindingContext = exercicio;

            // Exibe o DetalhesStackLayout e oculta o NovoExercicioLayout
            DetalhesStackLayout.IsVisible = true;
            NovoExercicioLayout.IsVisible = false;
        }
    }





    private List<DetalhesExercicio> GetDetalhesExercicio(Exercicios exercicio)
    {
        List<DetalhesExercicio> detalhes = new List<DetalhesExercicio>
        {
            // Adicione os detalhes do exerc�cio � lista de detalhes



            new DetalhesExercicio { Detalhe = "N�mero de S�ries: ",Valor =" "+exercicio.Nseries.ToString()},
            new DetalhesExercicio { Detalhe = "Peso (Kg): ", Valor = " "+exercicio.Peso.ToString()},
            new DetalhesExercicio { Detalhe = "Descri��o: ", Valor = " "+exercicio.Descricao }
        };

        return detalhes;
    }

    private async void ExcluirExercicioTapped(object sender, EventArgs e)
    {
        // Obt�m o exerc�cio selecionado
        var exercicio = (Exercicios)((Image)sender).BindingContext;

        // Confirma��o da exclus�o
        bool resposta = await DisplayAlert("Confirma��o", $"Deseja excluir o exerc�cio {exercicio.Nome}?", "Sim", "N�o");

        if (resposta)
        {
            // Remove o exerc�cio da lista
            listaexercicios.Remove(exercicio);

            // Exclui o exerc�cio do banco de dados
            await ExcluirExercicioDoDatabase(exercicio);
            // Atualiza a origem de dados da ListView
            ExerciciosListView.ItemsSource = null;
            ExerciciosListView.ItemsSource = listaexercicios;
        }
    }

    private async Task ExcluirExercicioDoDatabase(Exercicios exercicio)
    {
        string databaseFilename = "academia.db";
        string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseFilename);
        string connectionString = $"Data Source={databasePath}";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();

            // Execute o comando SQL para excluir o exerc�cio
            string deleteQuery = $"DELETE FROM exercicio WHERE id_exercicio = {exercicio.Id_exercicio}";
            using (SqliteCommand deleteCommand = new SqliteCommand(deleteQuery, connection))
            {
                int rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    // Exclus�o bem-sucedida, exiba um aviso
                    await DisplayAlert("Sucesso", "Exerc�cio exclu�do com sucesso.", "OK");
                }
                else
                {
                    // Nenhum registro foi exclu�do, exiba uma mensagem de aviso
                    await DisplayAlert("Aviso", "O exerc�cio n�o foi encontrado.", "OK");
                }
            }

            connection.Close();
        }
    }



}