/*
 * Adam Botens
 * 
 * Prototype of a simple Book Reader
 * 
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Book_Reader
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            VM = new ViewModel();
            DataContext = VM;
            BooksListView.ItemsSource = VM.Books;
            FilterListView.ItemsSource = VM.Authors;
            FillLinesPerPage();
            FillFontSizes();
            FillFontWeights();

        }
        public ViewModel VM { get; set; }
        /// <summary>
        /// Fills the font size combo box
        /// </summary>
        public void FillFontSizes()
        {
            for (int i = 12; i <= 40; i += 2)
            {
                FontSizeComboBox.Items.Add(i.ToString());
            }
        }
        /// <summary>
        /// Fills the font weights combo box
        /// </summary>
        public void FillFontWeights()
        {
            FontWeightComboBox.Items.Add("Bold");
            FontWeightComboBox.Items.Add("Normal");
            FontWeightComboBox.Items.Add("Thin");

        }
        /// <summary>
        /// Fills the Lines per page combo box
        /// </summary>
        public void FillLinesPerPage()
        {
            for (int i = 10; i <= 50; i += 10)
            {
                LinesPerPageComboBox.Items.Add(i.ToString());
            }
        }
        /// <summary>
        /// Fills out the pages of the current selected book. Uses SplitPages function inside Book class.
        /// </summary>
        /// <param name="selectedBook">Book that is currently selected.</param>
        /// 
        public void PopulatePages(Book selectedBook)
        {
            string fileContents;
            var assembly = Assembly.GetExecutingAssembly();
            var fileNames = assembly.GetManifestResourceNames();
            var resourceName = fileNames.Single(str => str.EndsWith(selectedBook.FileName));

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        fileContents = reader.ReadToEnd();
                    }
                }
                else
                {
                    fileContents = "error opening file";
                    return;
                }

                var result = int.TryParse(LinesPerPageComboBox.SelectedItem.ToString(), out int linesPerPage);
                string[] stringsArray = fileContents.Split('\n');
                BookReaderBlock.Text = selectedBook.SplitPages(stringsArray, linesPerPage).ToString();

            }
        }
        /// <summary>
        /// Event handler for the About selection in the Menu
        /// </summary>
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("This application was developed by Adam Botens.\n Email: abotens@my.apsu.edu", 
                "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        /// <summary>
        /// Event handler for the Exit selection in the Menu
        /// </summary>
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            
        }
        /// <summary>
        /// Event handler for the Next Page button. Makes use of the FlipPage function from the Book class.
        /// </summary>
        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksListView.SelectedIndex != -1)
            {
                Book? selectedBook = BooksListView.SelectedItem as Book;
                if (selectedBook == null)
                {
                    return;
                }
                BookReaderBlock.Text = selectedBook.FlipPage(1).ToString();
            }
            else
            {
                BookReaderBlock.Text = "No Book has been selected";

            }

        }
        /// <summary>
        /// Event handler for the Page One button. Makes use of the FlipPage function from the Book class.
        /// </summary>
        private void PageOneButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksListView.SelectedIndex != -1)
            {
                Book? selectedBook = BooksListView.SelectedItem as Book;
                if (selectedBook == null)
                {
                    return;
                }
                BookReaderBlock.Text = selectedBook.FlipPage(0).ToString();
            }
            else
            {
                BookReaderBlock.Text = "No Book has been selected";

            }
        }
        /// <summary>
        /// Event handler for the Previous Page button. Makes use of the FlipPage function from the Book class.
        /// </summary>
        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksListView.SelectedIndex != -1)
            {
                Book? selectedBook = BooksListView.SelectedItem as Book;
                if (selectedBook == null)
                {
                    return;
                }
                BookReaderBlock.Text = selectedBook.FlipPage(-1).ToString();
            }
            else
            {
                BookReaderBlock.Text = "No Book has been selected";

            }
        }
        /// <summary>
        /// Event handler for the Reset selection in the Menu. Sets all values back to default.
        /// </summary>
        private void ResetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Reset to Default?", "Comfirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                BookReaderBlock.Text = string.Empty;
                BooksListView.SelectedIndex = -1;
                FilterListView.SelectedIndex = 0;
                FontTypeComboBox.SelectedIndex = 8;
                FontSizeComboBox.SelectedIndex = 3;
                FontWeightComboBox.SelectedIndex = 1;
                LinesPerPageComboBox.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Event handler for See Book on Project Gutenberg. Opens the current book's URL in a
        /// web browser.
        /// </summary>
        private void SeeBookMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Book? selectedBook = BooksListView.SelectedItem as Book;
            if (selectedBook != null)
            {
                string URL = selectedBook.URL;
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = URL,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("You have not selected a book.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Event handler for Book List View. Calls PopulatePages function for the selected Title.
        /// </summary>
        private void BooksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Book? selectedBook = BooksListView.SelectedItem as Book;
            if (selectedBook != null)
            {
                PopulatePages(selectedBook);
            }

        }
        /// <summary>
        /// Event handler for Filter by Author list View. Changes Book List view to only list Titles
        /// from selected Author.
        /// </summary>
        private void FilterListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortedSet<Book> books = new();
            if (FilterListView.SelectedIndex == 0)
            {
                BooksListView.ItemsSource = VM.Books;
            }
            else
            {
                if (VM.Books == null)
                {
                    return;
                }
                foreach (Book book in VM.Books)
                {
                    if (FilterListView.SelectedItem != null)
                    { 
                        if (book.Author.Equals(FilterListView.SelectedItem.ToString()))
                        {
                            books.Add(book);
                        }
                    }
                }
                BooksListView.ItemsSource = books;
                BookReaderBlock.Text = "";
            }
        }
        /// <summary>
        /// Event handler for Lines Per Page Combo Box. If changed, repopulates the pages from selected book
        /// and goes back to page one.
        /// </summary>
        private void LinesPerPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Book? selectedBook = BooksListView.SelectedItem as Book;
            if (selectedBook != null)
            {
                PopulatePages(selectedBook);
            }

        }
        /// <summary>
        /// Event handler for X button in top right corner. Checks to ensure if user really wishes to close.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you wish to exit?", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else
                e.Cancel = true;
        }
        /// <summary>
        /// ViewModel for Catalog that is created from BOOK_INDEX.txt, list of authors, and list of books.
        /// Uses CreateCatalog to create the catalog and CreateAuthorsAndBooks to create lists of both.
        /// </summary>
        public class ViewModel
        {
            private SortedSet<Author>? _authors;
            private SortedSet<Book>? _books;
            private string _bookCatalog = "";
            public SortedSet<Author>? Authors
            {
                get { return _authors; }
                set { _authors = value; }
            }
            public SortedSet<Book>? Books
            {
                get { return _books; }
                set
                {
                    _books = value;
                }
            }
            public string BookCatalog 
            { 
                get { return _bookCatalog; }
                set { _bookCatalog = value; }
            }
            public ViewModel()
            {
                CreateCatalog();
                CreateAuthorsAndBooks();
            }
            public void CreateAuthorsAndBooks()
            {
                string? title;
                string? author;
                string? fileName;
                string? url;
                var books = new SortedSet<Book>();
                var authors = new SortedSet<Author>();
                authors.Add(new Author("All"));
                using (StringReader strReader = new StringReader(BookCatalog))
                {
                    while (true)
                    {
                        if ((title = strReader.ReadLine()) == null)
                        {
                            break;
                        }
                        else if ((author = strReader.ReadLine()) == null)
                        {
                            break;
                        }
                        else if ((fileName = strReader.ReadLine()) == null)
                        {
                            break;
                        }
                        else if ((url = strReader.ReadLine()) == null)
                        {
                            break;
                        }
                        Debug.WriteLine(books);
                        books.Add(new Book(title, author, fileName, url));
                        authors.Add(new Author(author));
                    }
                }
                Books = books;
                Authors = authors;
            }
            public void CreateCatalog()
            {
                string fileContents;
                var assembly = Assembly.GetExecutingAssembly();
                var fileNames = assembly.GetManifestResourceNames();
                var resourceName = fileNames.Single(str => str.EndsWith("BOOK_INDEX.txt"));

                using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            fileContents = reader.ReadToEnd();
                        }
                    }
                    else
                    {
                        fileContents = "error opening file";
                    }
                }
                Debug.WriteLine(fileContents);
                BookCatalog = fileContents;
            }

        }
        /// <summary>
        /// Class for Authors. Holds names. Uses IComparable interface to sort.
        /// </summary>
        public class Author : IComparable
        {
            private string _name;
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
            public Author(string name)
            {
                _name = name;
            }
            public int CompareTo(object? obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                if (obj.ToString() == "All")
                {
                    return 1;
                }
                var other = (Author)obj;
                var nameComparison = Name.CompareTo(other.Name);

                return nameComparison;
            }
            public override string ToString()
            {
                return $"{Name}";
            }
        }
        /// <summary>
        /// Class for Books. Holds list of Pages, current page number, and info for books. 
        /// Uses IComparable interface for sorting. Makes use of SplitPage and FLipPage functions.
        /// </summary>
        public class Book : IComparable
        {
            private List<Page> _pages = new List<Page>();
            private int _currentPageNumber;
            private string _title;
            private string _author;
            private string _fileName;
            private string _url;

            public List<Page> Pages
            {
                get { return _pages; }
                set { _pages = value; }
            }

            public int CurrentPageNumber
            {
                get { return _currentPageNumber; }
                set { _currentPageNumber = value; }
            }

            public string Title
            {
                get { return _title; }
                set { _title = value; }
            }
            public string Author
            {
                get { return _author; }
                set { _author = value; }
            }
            public string FileName
            {
                get { return _fileName; }
                set
                {
                    _fileName = value;
                }
            }
            public string URL
            {
                get { return _url; }
                set { _url = value; }
            }
            public Book(string title, string author, string fileName, string url)
            {
                _title = title;
                _author = author;
                _fileName = fileName;
                _url = url;
            }
            public int CompareTo(object? obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                var other = (Book)obj;
                var titleComparison = Title.CompareTo(other.Title);

                if (titleComparison == 0)
                    return 1;

                return titleComparison;
            }
            /// <summary>
            /// Function used for splitting pages by lines per page.
            /// </summary>
            /// <param name="contents">Contents of the currently selected book.</param>
            /// <param name="linesPerPage">Number of lines per page from combo box.</param>
            /// <returns>First Page of the book.</returns>
            public Page SplitPages(string[] contents, int linesPerPage)
            {
                int cursor = 0;
                int nextBound = linesPerPage;
                Pages.Clear();
                
                for (int i = 0; i <= contents.Length / linesPerPage + 1; i++)
                {
                    Pages.Add(new Page(contents, nextBound, cursor));
                    cursor += linesPerPage;
                    nextBound += linesPerPage;
                }
                return Pages[0];
            }
            /// <summary>
            /// Function used for flipping pages.
            /// </summary>
            /// <param name="flipPage">-1 for previous page, 0 for first page, and 1 for next page.</param>
            /// <returns>Returns page based on user selection.</returns>
            public Page FlipPage(int flipPage)
            {
                if (flipPage == -1)
                {
                    if (CurrentPageNumber == 0)
                        return Pages[0];
                    return Pages[--CurrentPageNumber];
                }
                else if (flipPage == 0)
                {
                    CurrentPageNumber = 0;
                    return Pages[0];
                }
                else if (flipPage == 1)
                {
                    if (CurrentPageNumber == Pages.Count - 1)
                        return Pages[CurrentPageNumber];
                    return Pages[++CurrentPageNumber];
                }
                else
                    return Pages[CurrentPageNumber];
            }
            public override string ToString()
            {
                return $"{Title}";
            }

        }
        /// <summary>
        /// Page class that holds the contents of the book.
        /// </summary>
        public class Page
        {
            private string _pageContents = "";
            public string PageContents
            {
                get { return _pageContents; }
                set { _pageContents = value; }
            }
            /// <summary>
            /// Constructs a Page. This is only called from SplitPage function inside Book class.
            /// </summary>
            /// <param name="contents">Content of selected book</param>
            /// <param name="nextBound">Keeps track of right bound.</param>
            /// <param name="cursor">Keeps track of left bound.</param>
            public Page(string[] contents, int nextBound, int cursor)
            {
                if (nextBound > contents.Length) 
                { 
                    nextBound = contents.Length;
                }
                for (int i = cursor; i < nextBound; i++)
                {
                    _pageContents += contents[i];
                    _pageContents += "\n";
                }
            }

            public override string ToString()
            {
                return $"{PageContents}";
            }
        }

    }

}
