namespace Minimum
{
    public class ProjectGraphCreator
    {
        public string[][] Create()
        {
            var g =new string[2][]; 
            g[0]=new string[2];
            var nameIdx = (int)ProjectGraph.MaxMinDataStructureIndexes.CsprojName;
            g[0][nameIdx]="TagExample.csproj";
            g[0][(int)ProjectGraph.MaxMinDataStructureIndexes.CsprojPath]="c:/MyGit/DevTools/MinimumTests/sones/Applications/TagExample";
            
            g[1]=new string[2];
            g[1][nameIdx]="IGraphDB.csproj";
            g[1][(int)ProjectGraph.MaxMinDataStructureIndexes.CsprojPath]="c:/MyGit/DevTools/MinimumTests/sones/GraphDB/IGraphDB";
            return g;
        }
    }
}
