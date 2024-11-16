using SnippetsLibraryWebApp.Repository;

namespace SnippetsLibraryWebApp.Controllers
{
    public class SavedSnippetsController
    {
        private readonly SnippetRepository _snippetRepository;

        public SavedSnippetsController(SnippetRepository snippetRepository)
        {
            _snippetRepository = snippetRepository;
        }
    }
}
