using SnippetsLibraryWebApp.Repository;

namespace SnippetsLibraryWebApp.Controllers
{
    public class SnippetDetailsController
    {
        private readonly SnippetRepository _snippetRepository;

        public SnippetDetailsController(SnippetRepository snippetRepository)
        {
            _snippetRepository = snippetRepository;
        }
    }
}
