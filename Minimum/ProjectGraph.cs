namespace Minimum
{
    public static class ProjectGraph
    {
        //TO-DO graph worked out manually for now
        public enum MaxMinDataStructureIndexes
        {
            CsprojName = 0,
            CsprojPath = 1
        }
        
        public static int GraphLength => Graph.Length;
        private static string[][] _graph;
        public static string[][] Graph
        {
            get
            {
                if(_graph==null)_graph = Create();
                return _graph;
            }
        }
        
        public static string[][] Create()
        {
            var numberProjects = 2;
            var g = new string[numberProjects][];
            for (int i = 0; i < numberProjects; i++) g[i]=new string[2];
            
            var nameIdx = (int)MaxMinDataStructureIndexes.CsprojName;
            var projectPathIdx = (int)MaxMinDataStructureIndexes.CsprojPath; 
            g[0][nameIdx] = "TagExample.csproj";
            g[0][projectPathIdx] = "c:/MyGit/DevTools/MinimumTests/sones/Applications/TagExample";

            g[1][nameIdx] = "IGraphDB.csproj";
            g[1][projectPathIdx] = "c:/MyGit/DevTools/MinimumTests/sones/GraphDB/IGraphDB";
            return g;
        }
    }
}
