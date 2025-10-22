namespace DatabaseOperationsWithEFCore.Models
{
    public class Language
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        /* Since a book can be published in multiple languages. Hence, we need to use One to many relationship.
        *  We need to add foreign key (Primary key of Language)
        *  There are 2 steps to do this:
        *  Step 1:
        *       A. Add a property of type Language in Book class (line: 24 in Book.cs)
        *       B. Add a property of type int in Book class which will hold the LanguageId (Line: 26 in Book.cs)
        *       
        *  Step 2:
        *       A. Add a property of type ICollection<Book> in Language class (line: 20 in Language.cs)
        */
        public virtual ICollection<Book> Books { get; set; }
    }
}
