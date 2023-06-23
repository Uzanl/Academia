using Academia.Classes;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace Academia;

public partial class MainPage : FlyoutPage
{
    List<Treino> Listatreino { get; set; } = new List<Treino>();
    List<Exercicios> Listaexercicios { get; set; } = new List<Exercicios>();

    List<Exercicios> ListaExerciciosTreinos { get; set; } = new List<Exercicios>();
    private ObservableCollection<Exercicios> exerciciosSelecionados = new ObservableCollection<Exercicios>();

   // private ObservableCollection<Exercicios> exerciciosRelacionados = new ObservableCollection<Exercicios>();

    private void AddRange<T>(ObservableCollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }


    public MainPage()
    {
        InitializeComponent();

       // ExerciciosRelacionados = new ObservableCollection<Exercicios>();

        string databaseFilename = "academia.db";
        string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseFilename);
        string connectionString = $"Data Source={databasePath}";





        using (SqliteConnection connection = new(connectionString))
        {
            connection.Open();




            string createTableQuery = "CREATE TABLE IF NOT EXISTS treino (id_treino INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT NOT NULL)";

            // Criação da tabela "treino"
            using (SqliteCommand command = new(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            //Criação da tabela "exercicio"
            string createExercicioTableQuery = "CREATE TABLE IF NOT EXISTS exercicio (id_exercicio INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT, nseries INTEGER, repeticao INTEGER, peso INTEGER, descricao TEXT)";

            using (SqliteCommand createExercicioTableCommand = new(createExercicioTableQuery, connection))
            {
                createExercicioTableCommand.ExecuteNonQuery();
            }


            // Criação da tabela "exercicios_treino"
            string createExerciciosTreinoTableQuery = "CREATE TABLE IF NOT EXISTS exercicios_treino (id_treino INTEGER, id_exercicio INTEGER, FOREIGN KEY (id_treino) REFERENCES treino(id_treino) ON UPDATE CASCADE ON DELETE CASCADE, FOREIGN KEY (id_exercicio) REFERENCES exercicio(id_exercicio) ON UPDATE CASCADE ON DELETE CASCADE)";

            using (SqliteCommand createExerciciosTreinoTableCommand = new(createExerciciosTreinoTableQuery, connection))
            {
                createExerciciosTreinoTableCommand.ExecuteNonQuery();
            }

            connection.Close();
        }



        // Obtenha os exercícios do banco de dados ou qualquer outra fonte de dados
        Listaexercicios = GetExerciciosFromDatabase();
        Listatreino = GetTreinosFromDatabase();


        TreinosPesqListView.ItemsSource = Listatreino;
        // Defina a origem de dados da ListView
        ExerciciosListView.ItemsSource = Listaexercicios;

        ExerciciosSelecionadosListView.ItemsSource = exerciciosSelecionados;

        // Preencha o Picker com os exercícios cadastrados

     

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

        // Realize a lógica de consulta no banco de dados com o filtro LIKE
        List<Exercicios> searchResults = GetExerciciosLikeFromDatabase(searchText);

        // Atualize a fonte de dados da ListView com os resultados da pesquisa
        ExerciciosListView.ItemsSource = searchResults;
    }

    private void ExercicioTapped(object sender, EventArgs e)
    {
        if (sender is TextCell selectedCell && selectedCell.BindingContext is Exercicios exercicio)
        {
            exerciciosSelecionados.Add(exercicio);

            // Atualize a origem de dados do ListView de ExerciciosSelecionados
            ExerciciosSelecionadosListView.ItemsSource = exerciciosSelecionados;
        }
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

                        Exercicios exercicio = new Exercicios(id, nome, nseries, repeticao, peso, descricao);
                        exercicios.Add(exercicio);
                    }
                }
            }

            connection.Close();
        }

        return exercicios;
    }

    private void NovoTreinoClicked(object sender, EventArgs e)
    {
        // Cria a nova página que deseja exibir no detalhe
        ContentPage novaPagina = new FrmTreino();

        // Obtém a instância do FlyoutPage atual
        FlyoutPage flyoutPage = (FlyoutPage)Application.Current.MainPage;

        // Define a nova página como o conteúdo principal do FlyoutPage
        flyoutPage.Detail = new NavigationPage(novaPagina);

        // Fecha o menu lateral (flyout)
        flyoutPage.IsPresented = false;
    }

    private void NovoExercicioClicked(object sender, EventArgs e)
    {
        // Cria a nova página que deseja exibir no detalhe
        ContentPage novaPagina = new FrmExercicio();

        // Obtém a instância do FlyoutPage atual
        FlyoutPage flyoutPage = (FlyoutPage)Application.Current.MainPage;

        // Define a nova página como o conteúdo principal do FlyoutPage
        flyoutPage.Detail = new NavigationPage(novaPagina);

        // Fecha o menu lateral (flyout)
        flyoutPage.IsPresented = false;
    }

    private void NovoTreinoeClicked(object sender, EventArgs e)
    {
        NovoTreinoLayout.IsVisible = !NovoTreinoLayout.IsVisible;
    }

    private async void VoltarClicked(object sender, EventArgs e)
    {
        // Limpa a pilha de navegação e define a MainPage como a nova página inicial
        await (App.Current.MainPage as FlyoutPage).Detail.Navigation.PopToRootAsync();
        Application.Current.MainPage = new MainPage();
    }

    private void AdicionarTreinoClicked(object sender, EventArgs e)
    {
        //ExerciciosPicker.IsVisible = true;
        NovoTreinoLayout.IsVisible = true;
    }


    private void AdicionarExercicioSelecionado(object sender, EventArgs e)
    {
        var exercicio = (sender as TextCell)?.BindingContext as Exercicios;
        if (exercicio != null)
        {
            exerciciosSelecionados.Add(exercicio);
            ExerciciosListView.SelectedItem = null;
        }
    }

    private void RemoverExercicioTapped(object sender, EventArgs e)
    {
        var exercicio = (sender as TextCell)?.BindingContext as Exercicios;
        if (exercicio != null)
        {
            exerciciosSelecionados.Remove(exercicio);
        }
    }


    private List<Treino> GetTreinosFromDatabase()
    {
        List<Treino> treinos = new List<Treino>();

        string databaseFilename = "academia.db";
        string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseFilename);
        string connectionString = $"Data Source={databasePath}";

        using (SqliteConnection connection = new SqliteConnection($"Data Source={databasePath}"))
        {
            connection.Open();

            string query = "SELECT * FROM treino";

            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string nome = reader.GetString(1);

                        Treino treino = new Treino(id, nome);
                        treinos.Add(treino);
                    }
                }
            }

            connection.Close();
        }

        return treinos;
    }

    private List<Treino> GetTreinosLikeFromDatabase(string nomeTreino)
    {
        List<Treino> treinos = new List<Treino>();

        string databaseFilename = "academia.db";
        string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseFilename);
        string connectionString = $"Data Source={databasePath}";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string query = $"SELECT * FROM treino WHERE nome LIKE '{nomeTreino}%'";

            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string nome = reader.GetString(1);

                        Treino treino = new Treino(id, nome);
                        treinos.Add(treino);
                    }
                }
            }

            connection.Close();
        }

        return treinos;
    }

    private void OnSearchTreinoTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue;

        // Realize a lógica de consulta no banco de dados com o filtro LIKE
        List<Treino> searchResults = GetTreinosLikeFromDatabase(searchText);

        // Atualize a fonte de dados da ListView com os resultados da pesquisa
        TreinosPesqListView.ItemsSource = searchResults;
    }



    private void CadastrarTreino(object sender, EventArgs e)
    {

        string databaseFilename = "academia.db";
        string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseFilename);
        string connectionString = $"Data Source={databasePath}";

        string nomeTreino = TxtCadNomeTreino.Text.Trim();

        // Verificar se o nome do treino foi fornecido
        if (string.IsNullOrEmpty(nomeTreino))
        {
            DisplayAlert("Erro", "O nome do treino é obrigatório", "OK");
            return;
        }

        // Verificar se pelo menos um exercício foi selecionado
        if (exerciciosSelecionados.Count == 0)
        {
            DisplayAlert("Erro", "Selecione pelo menos um exercício", "OK");
            return;
        }

        // Inserir dados na tabela "treino" e obter o ID do treino inserido
        string insertTreinoQuery = $"INSERT INTO treino (nome) VALUES ('{nomeTreino}'); SELECT last_insert_rowid()";

        long treinoId;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(insertTreinoQuery, connection))
            {
                treinoId = (long)command.ExecuteScalar();
            }

            // Inserir dados na tabela "exercicios_treino" para cada exercício selecionado
            foreach (Exercicios exercicio in exerciciosSelecionados)
            {
                string insertExerciciosTreinoQuery = $"INSERT INTO exercicios_treino (id_treino, id_exercicio) VALUES ({treinoId}, {exercicio.Id_exercicio})";

                using (SqliteCommand command = new SqliteCommand(insertExerciciosTreinoQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
        }

        DisplayAlert("Sucesso", "Treino cadastrado com sucesso", "OK");

        // Obtenha os exercícios do banco de dados ou qualquer outra fonte de dados
        List<Treino> listatreino = GetTreinosFromDatabase();

        // Defina a origem de dados da ListView
        TreinosPesqListView.ItemsSource = listatreino;

        // Limpar campos e reinicializar a lista de exercícios selecionados
        TxtCadNomeTreino.Text = string.Empty;
        exerciciosSelecionados.Clear();
        ExerciciosSelecionadosListView.ItemsSource = null;
    }


    private List<Exercicios> ConsultarExerciciosRelacionados(int treinoId)
    {
        List<Exercicios> exerciciosRelacionados = new List<Exercicios>();

        string databaseFilename = "academia.db";
        string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseFilename);
        string connectionString = $"Data Source={databasePath}";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string query = $"SELECT e.* FROM exercicio e JOIN exercicios_treino et ON e.id_exercicio = et.id_exercicio WHERE et.id_treino = {treinoId}";

            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    //exerciciosRelacionados = new ObservableCollection<Exercicios>();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string nome = reader.GetString(1);
                        int nseries = reader.GetInt32(2);
                        int repeticao = reader.GetInt32(3);
                        int peso = reader.GetInt32(4);
                        string descricao = reader.GetString(5);

                        Exercicios exercicio = new Exercicios(id, nome, nseries, repeticao, peso, descricao);
                        exerciciosRelacionados.Add(exercicio);
                    }
                }
            }

            connection.Close();
        }

        return exerciciosRelacionados;
    }

    private void TreinoTapped(object sender, ItemTappedEventArgs e)
    {
       // ListView listView = (ListView)sender;
        Treino treino = (Treino)e.Item;
      

        // Inverter a visibilidade da lista de exercícios relacionados
      // ExerciciosTreinoListView.IsVisible = !ExerciciosTreinoListView.IsVisible;

       
            // Consultar os exercícios relacionados
            List<Exercicios> exerciciosRelacionados = ConsultarExerciciosRelacionados(treino.Id_treino);

            // Definir a origem de dados para a lista de exercícios
            ExerciciosTreinoListView.ItemsSource = exerciciosRelacionados;
       
    }

    private async void ExcluirExercicioTreinoTapped(object sender, EventArgs e)
    {
        // Obtém o exercício selecionado
        var exercicio = (Exercicios)((Image)sender).BindingContext;

        // Confirmação da exclusão
        bool resposta = await DisplayAlert("Confirmação", $"Deseja excluir o exercício {exercicio.Nome}?", "Sim", "Não");

        if (resposta)
        {
            // Remove o exercício da lista
            ListaExerciciosTreinos.Remove(exercicio);

            // Exclui o exercício do banco de dados
            await ExcluirExercicioTreinoDoDatabase(exercicio);
            // Atualiza a origem de dados da ListView
            ExerciciosTreinoListView.ItemsSource = null;
            ExerciciosTreinoListView.ItemsSource = ListaExerciciosTreinos;
        }
    }

    private async Task ExcluirExercicioTreinoDoDatabase(Exercicios exercicio)
    {
        string databaseFilename = "academia.db";
        string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseFilename);
        string connectionString = $"Data Source={databasePath}";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();

            // Execute o comando SQL para excluir o exercício
            string deleteQuery = $"DELETE FROM exercicios_treino WHERE id_exercicio = {exercicio.Id_exercicio}";
            using (SqliteCommand deleteCommand = new SqliteCommand(deleteQuery, connection))
            {
                int rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    // Exclusão bem-sucedida, exiba um aviso
                    await DisplayAlert("Sucesso", "Exercício excluído com sucesso.", "OK");
                }
                else
                {
                    // Nenhum registro foi excluído, exiba uma mensagem de aviso
                    await DisplayAlert("Aviso", "O exercício não foi encontrado.", "OK");
                }
            }

            connection.Close();
        }
    }





}

