using SnippetsLibraryWebApp.Repository;

namespace SnippetsLibraryWebApp.Controllers
{
    public class PersonalSnippetsController
    {
        private readonly SnippetRepository _snippetRepository;

        public PersonalSnippetsController(SnippetRepository snippetRepository)
        {
            _snippetRepository = snippetRepository;
        }
    }
}
