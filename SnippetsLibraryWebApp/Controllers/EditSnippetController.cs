using SnippetsLibraryWebApp.Repository;

namespace SnippetsLibraryWebApp.Controllers
{
    public class EditSnippetController
    {
        private readonly SnippetRepository _snippetRepository;

        public EditSnippetController(SnippetRepository snippetRepository)
        {
            _snippetRepository = snippetRepository;
        }

    }
}
