﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace adamic_adar
{
    class graph
    {
        public int N; // number of nodes
        public class Node
        {
            public int[] freinds;
            public ArrayList f;
            public int degree;
            public double newMethod;
            public double score; // adamic adar score
            public double  jaccard;
            public int preferential;
            public double  HDI , HPI;
            public Node(int freind)
            {
                freinds = new int[freind];
                f = new ArrayList();
                 degree = 0;
            }
        }

        public Node[] nodes;
        public class  AucScore
        {
           public  double adaminc, jaccard, newmethod, hdi, hpi, preferetial;
        }

        public class idcompare : IComparer
        {
           public  int  Compare(object  x , object   y)
            {
                

                if (((Node) x).score  > ((Node) y).score )
                    return Convert.ToInt16( ((Node)x).score - ((Node)y).score);
                else
                    return Convert.ToInt16 ( ((Node)y).score  - ((Node)x).score) ;

            }
        }

        public graph(int numOfNodes, int MaxNodeDegree)
        {
            
            N = numOfNodes;
            nodes = new Node[numOfNodes];
            for (int i = 1; i < numOfNodes; i++)
                nodes[i] = new Node(MaxNodeDegree);
        }

        public void loadfromfile(string filename)
        {
            {
                bool e = true;
                int j = 1;
                FileStream f = File.OpenRead(filename );
                TextReader r = new StreamReader(f);
               
                while (e)
                {
                    string s = r.ReadLine();
                    if ((s == null))   // end of file
                    {
                        e = false;
                        break; 
                    }

                    bool check = true;
                    int l = 0;
                    int[] index = new int[10];

                    for (int i = 0; i < s.Length; i++)    // counting numbers in each line of the file
                    {
                        if (s[i] == ' ')
                            check = true;
                        if (check & s[i] != ' ')
                        {
                            index[l++] = i;
                            check = false;
                        }
                    }
                    index[l] = s.Length;
                    nodes[j].degree = l - 1;
                    for (int i = 0; i < index.Length - 1; i++)   // reading numbers
                        if (index[i + 1] != 0)
                            nodes[j].freinds[i] = Convert.ToInt32(s.Substring(index[i], (index[i + 1] - index[i])).Trim());
                    
                    j++;
                }

                r.Close();
                f.Close();
                
            }

        }


        public int loadfromfileEg(string filename)
        {
            {
                bool e = true;
                int j = 1;
                int n = 0;
                FileStream f = File.OpenRead(filename);
                TextReader r = new StreamReader(f);

                while (e)
                {
                    string s =  r.ReadLine();
                    
                    if ((s == null))   // end of file
                    {
                        e = false;
                        break;
                    }
                    n++;
                    s = s.Replace("\t", "            ") + "               ";
                    bool check = true;
                    int l = 0;
                    int[] index = new int[10];

                    for (int i = 0; i < s.Length; i++)    // counting numbers in each line of the file
                    {
                        if (s[i] == ' ')
                            check = true;
                        if (check & s[i] != ' ')
                        {
                            index[l++] = i;
                            check = false;
                        }
                    }
                    index[l] = s.Length+1;
                    
                  //  for (int i = 0; i < index.Length - 1; i++)   // reading numbers
                  //      if (index[i + 1] != 0)
                  //      {
                    int ind = Convert.ToInt32(s.Substring(index[0], (index[1] - index[0])).Trim());
                    int ind2 = Convert.ToInt32(s.Substring(index[1], 8).Trim());
                    if (!  (nodes[ind].freinds.Contains( (ind2))))
                    {
                        nodes[ind].degree ++;
                        nodes[ind].freinds[0] = ind;
                        nodes[ind].freinds[nodes[ind].degree] = Convert.ToInt32(s.Substring(index[1], 8).Trim());
                    }
                    if(! (nodes[ind2].freinds.Contains(ind )))
                        {
                        nodes[ind2].degree ++;
                        nodes[ind2].freinds[0] = ind2;
                        nodes[ind2].freinds[nodes[ind2].degree] = ind;

                    }
                           
                  //      }

                            j++;
                }

                r.Close();
                f.Close();
                return n;

            }

        }

        public void preferentialATCH(ref ArrayList p, int u)
        {
            foreach (graph.Node n in nodes)
                if (n != null) 
                n.preferential = 0;

            for (int i = 1; i <= nodes[u].degree; i++)
            {
                if (nodes[u].freinds[i] < N)
                    for (int j = 1; j <= nodes[(nodes[u].freinds[i])].degree; j++)
                    {

                        if (nodes[nodes[u].freinds[i]].freinds[j] < N)
                        {
                            
                            bool exist = false;
                            for (int g = 1; g < nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds.Length; g++)
                                if (nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds[g] == u)
                                    exist = true;
                            if ((u != nodes[nodes[u].freinds[i]].freinds[j]) && !exist)
                                nodes[nodes[nodes[u].freinds[i]].freinds[j]].preferential = nodes[nodes[nodes[u].freinds[i]].freinds[j]].degree   * nodes[u].degree ;

                        }
                    }
            }

            for (int i = 1; i < N; i++)
                if (nodes[i].preferential  > 0)
                {
                  
                    p.Add(nodes[i]);

                }
            Node temp = new Node(10);
            for (int i = 0; i < p.Count; i++)
                for (int j = i; j < p.Count; j++)
                    if (((Node)p[i]).preferential  < ((Node)p[j]).preferential )
                    {
                        temp = (Node)p[i];
                        p[i] = p[j];
                        p[j] = temp;

                    }


        }

        public void HPI(ref ArrayList p, int u)
        {
            foreach (graph.Node n in nodes)
                if (n != null)
                    n.HPI  = 0;
            for (int i = 1; i <= nodes[u].degree; i++)
            {
                if (nodes[u].freinds[i] < N)
                    for (int j = 1; j <= nodes[(nodes[u].freinds[i])].degree; j++)
                    {

                        if (nodes[nodes[u].freinds[i]].freinds[j] < N)
                        {
                            int exp = 0;
                            bool exist = false;
                            for (int g = 1; g < nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds.Length; g++)
                                if (nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds[g] == u)
                                    exist = true;
                            if ((u != nodes[nodes[u].freinds[i]].freinds[j]) && !exist)
                                exp = 1;

                            nodes[nodes[nodes[u].freinds[i]].freinds[j]].HPI += exp;


                        }
                    }
            }

            for (int i = 1; i < N; i++)
                if (nodes[i].HPI > 0)
                {
                    if (nodes[u].degree < nodes[i].degree)
                        nodes[i].HPI = ((nodes[i].HPI) / (nodes[u].degree));
                    else
                        nodes[i].HPI = ((nodes[i].HPI) / (nodes[i].degree));
                    p.Add(nodes[i]);

                }
            Node temp = new Node(10);
            for (int i = 0; i < p.Count; i++)
                for (int j = i; j < p.Count; j++)
                    if (((Node)p[i]).HPI < ((Node)p[j]).HPI)
                    {
                        temp = (Node)p[i];
                        p[i] = p[j];
                        p[j] = temp;

                    }


        }

        public void HDI(ref ArrayList p, int u)
        {
            foreach (graph.Node n in nodes)
                if (n != null)
                    n.HDI  = 0;
            for (int i = 1; i <= nodes[u].degree; i++)
            {
                if (nodes[u].freinds[i] < N)
                    for (int j = 1; j <= nodes[(nodes[u].freinds[i])].degree; j++)
                    {

                        if (nodes[nodes[u].freinds[i]].freinds[j] < N)
                        {
                            int  exp = 0;
                            bool exist = false;
                            for (int g = 1; g < nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds.Length; g++)
                                if (nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds[g] == u)
                                    exist = true;
                            if ((u != nodes[nodes[u].freinds[i]].freinds[j]) && !exist)
                                exp = 1;

                            nodes[nodes[nodes[u].freinds[i]].freinds[j]].HDI  += exp;


                        }
                    }
            }

            for (int i = 1; i < N; i++)
                if (nodes[i].HDI  > 0)
                {
                    if(nodes[u].degree > nodes[i].degree )
                       nodes[i].HDI  = ((nodes[i].HDI ) / (nodes[u].degree ));
                    else
                        nodes[i].HDI = ((nodes[i].HDI) / (nodes[i].degree));
                    p.Add(nodes[i]);

                }
            Node temp = new Node(10);
            for (int i = 0; i < p.Count; i++)
                for (int j = i; j < p.Count; j++)
                    if (((Node)p[i]).HDI  < ((Node)p[j]).HDI )
                    {
                        temp = (Node)p[i];
                        p[i] = p[j];
                        p[j] = temp;

                    }


        }

        public void jaccard(ref ArrayList p, int u)
        {
            foreach (graph.Node n in nodes)
                if (n != null)
                    n.jaccard  = 0;
            for (int i = 1; i <= nodes[u].degree; i++)
            {
                if (nodes[u].freinds[i] < N)
                    for (int j = 1; j <= nodes[(nodes[u].freinds[i])].degree; j++)
                    {
                        
                        if (nodes[nodes[u].freinds[i]].freinds[j] < N)
                        {
                            double  exp = 0;
                            bool exist = false;
                            for (int g = 1; g < nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds.Length; g++)
                                if (nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds[g] == u)
                                    exist = true;
                            if ((u != nodes[nodes[u].freinds[i]].freinds[j]) && !exist)
                                exp = 1;

                            nodes[nodes[nodes[u].freinds[i]].freinds[j]].jaccard += exp;

                        }
                    }
            }

            for (int i = 1; i < N; i++)
                if (nodes[i].jaccard  > 0)
                {
                    nodes[i].jaccard = (nodes[i].jaccard / (nodes[u].degree  + nodes[i].degree  - nodes[i].jaccard));
                    p.Add(nodes[i]);

                }
            Node temp = new Node(10);
            for (int i = 0; i < p.Count; i++)
                for (int j = i; j < p.Count; j++)
                    if (((Node)p[i]).jaccard  < ((Node)p[j]).jaccard )
                    {
                        temp = (Node)p[i];
                        p[i] = p[j];
                        p[j] = temp;

                    }


        }

        public void adamicadar(ref ArrayList  p, int u)
        {
            foreach (graph.Node n in nodes)
                if (n != null)
                    if (n != null)
                n.score  = 0;
            for (int i = 1; i <= nodes[u].degree; i++)
            {
                if((nodes[u].freinds[i] < N) )
                for (int j = 1; j <= nodes[(nodes[u].freinds[i])].degree; j++)
                {
                        if (nodes[nodes[u].freinds[i]].freinds[j] < N)
                        {
                            double sc = 0;
                            bool exist = false;
                            for (int g = 1; g < nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds.Length; g++)
                                if (nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds[g] == u)
                                    exist = true;
                            if ((u != nodes[nodes[u].freinds[i]].freinds[j]) && !exist)
                                sc = 1 / (Math.Log10(Convert.ToDouble(nodes[nodes[u].freinds[i]].degree)));
                            nodes[nodes[nodes[u].freinds[i]].freinds[j]].score += sc;
                        }
                }
            }

            for (int i = 1; i < N; i++)
                if (nodes[i].score > 0)
                {
                    p.Add(nodes[i]);
                  
                }
            Node temp = new Node(10);
            for(int i = 0; i < p.Count; i++)
                for(int j = i; j < p.Count; j++)
                if(((Node)p[i]).score < ((Node)p[j]).score)
                {
                    temp = (Node) p[i];
                    p[i] = p[j];
                    p[j] = temp;

                }


        }

        public void newMethod(ref ArrayList p, int u)
        {
            foreach (graph.Node n in nodes)
                if (n != null)
                    n.newMethod  = 0;
            for (int i = 1; i <= nodes[u].degree; i++)
            {
                if ((nodes[u].freinds[i] < N))
                    for (int j = 1; j <= nodes[(nodes[u].freinds[i])].degree; j++)
                    {
                        if (nodes[nodes[u].freinds[i]].freinds[j] < N)
                        {
                            double sc = 0;
                            bool exist = false;
                            for (int g = 1; g < nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds.Length; g++)
                                if (nodes[nodes[nodes[u].freinds[i]].freinds[j]].freinds[g] == u)
                                    exist = true;
                            if ((u != nodes[nodes[u].freinds[i]].freinds[j]) && !exist)
                                sc = ( (Math.Log(Convert.ToDouble(nodes[u].degree),2 )) + (Math.Log(Convert.ToDouble(nodes[nodes[u].freinds[i]].degree),2) )) / (Math.Log10(Convert.ToDouble(nodes[nodes[u].freinds[i]].degree)));
                            nodes[nodes[nodes[u].freinds[i]].freinds[j]].newMethod  += sc;
                        }
                    }
            }

            for (int i = 1; i < N; i++)
                if (nodes[i].newMethod  > 0)
                {
                   
                    p.Add(nodes[i]);

                }
            
            Node temp = new Node(10);
            for (int i = 0; i < p.Count; i++)
                for (int j = i; j < p.Count; j++)
                    if (((Node)p[i]).newMethod  > ((Node)p[j]).newMethod )
                    {
                        temp = (Node)p[i];
                        p[i] = p[j];
                        p[j] = temp;

                    } 


        }

        public int  generate(ref Node[] test , int n , int f , int size, int rate)
        {
            test = new Node[n];
         //   nodes = new Node[n];
            for (int i = 1; i < n; i++)
                test[i] = new Node(f);

            Random rand = new Random();
            int x, y , tx , ty;
            int k = 1;
            while (k < ((n / 100) * rate))
            {

                x = rand.Next(1, n - 1);
                //Console.WriteLine(nodes[x].degree);
                if ((nodes[x].degree) > 0 )
                {
                  //  Console.WriteLine(x);
                    ty = rand.Next(1, nodes[x].degree);
                    y = nodes[x].freinds[ty];
                    if (nodes[y].freinds.Contains(x))
                    {
                        k++;
                        tx = Array.IndexOf(nodes[y].freinds, x);
                        for (int i = ty; i < nodes[x].degree - 1; i++)
                            nodes[x].freinds[i] = nodes[x].freinds[i + 1];
                        for (int j = tx; j < nodes[y].degree - 1; j++)
                            nodes[y].freinds[j] = nodes[y].freinds[j + 1];
                        test[x].freinds[++test[x].degree] = y;
                        test[y].freinds[++test[y].degree] = x;
                        test[y].freinds[0] = y;
                        test[x].freinds[0] = x;


                    }


                }
            }

            return k;
        }



        public void AUC(graph.Node[] Test , ref graph.AucScore auc ,int maxnode , int maxexe , int testSize )
        {
            Random rand = new Random();
            int R =  maxexe;
            int value;
            auc = new  graph. AucScore();
            int i = 1;
            int adc, jac, hdi, hpi, nem, pre;
            adc= jac= hdi= hpi= nem= pre=0;
            
            while (i < R)
            {
                again:
                
                int u = rand.Next(1, maxnode-1);

                ArrayList jaccardproposal = new ArrayList();
                ArrayList adamicproposal = new ArrayList();
                ArrayList preferetialpro = new ArrayList();
                ArrayList HDIpro = new ArrayList();
                ArrayList HPIpro = new ArrayList();
                ArrayList newMethodpro = new ArrayList();

                adamicadar(ref adamicproposal, u);
                jaccard(ref jaccardproposal, u);
                preferentialATCH(ref preferetialpro, u);
                HDI(ref HDIpro, u);
                HPI(ref HPIpro, u);
                newMethod(ref newMethodpro, u);




                

                i++;

                for (int j = 1; j < rand.Next(1, 10) ; j++)
                {
                    int counter = 0;
                    bool chk;
                    do
                    {
                        
                        if (counter == newMethodpro.Count)
                            goto again;
                        value = rand.Next(1, maxnode-1);
                        bool exist = false;
                        chk = false;
                        foreach (Node n in newMethodpro)
                            if (value == n.freinds[0])
                                exist = true;

                        if ((Test[value].degree > 0) & exist )
                            chk = true;
                        counter++;

                    } while (!chk);



                    int y = rand.Next(1, newMethodpro.Count / 2);

                    if (Test[((Node)newMethodpro[y]).freinds[0]].degree == 0)
                    {

                        if (nodes[value].newMethod > ((Node)newMethodpro[y]).newMethod)
                            auc.newmethod = auc.newmethod + 1;
                        else
                            if (nodes[value].newMethod == ((Node)newMethodpro[y]).newMethod)
                            auc.newmethod = auc.newmethod + 0.5;
                        else
                            nem++;


                        if (nodes[value].jaccard > ((Node)newMethodpro[y]).jaccard)
                            auc.jaccard = auc.jaccard + 1;
                        else
                            if (nodes[value].jaccard == ((Node)newMethodpro[y]).jaccard)
                            auc.jaccard = auc.jaccard + 0.5;
                        else
                            jac++;


                        if (nodes[value].HDI > ((Node)newMethodpro[y]).HDI)
                            auc.hdi = auc.hdi + 1;
                        else
                            if (nodes[value].HDI == ((Node)newMethodpro[y]).HDI)
                            auc.hdi = auc.hdi + 0.5;
                        else
                            hdi++;


                        if (nodes[value].preferential > ((Node)newMethodpro[y]).preferential)
                            auc.preferetial = auc.preferetial + 1;
                        else
                            if (nodes[value].preferential == ((Node)newMethodpro[y]).preferential)
                            auc.preferetial = auc.preferetial + 0.5;
                        else
                            pre++;

                        if (nodes[value].HPI > ((Node)newMethodpro[y]).HPI)
                            auc.hpi = auc.hpi + 1;
                        else
                            if (nodes[value].HPI == ((Node)newMethodpro[y]).HPI)
                            auc.hpi = auc.hpi + 0.5;
                        else
                            hpi++;

                        if (nodes[value].score > ((Node)newMethodpro[y]).score)
                            auc.adaminc = auc.adaminc + 1;
                        else
                            if (nodes[value].score == ((Node)newMethodpro[y]).score)
                            auc.adaminc = auc.adaminc + 0.5;
                        else
                            adc ++;
                    }
                    else
                        j--;

                    i++;


                }
                Console.Clear();
                Console.WriteLine(i);
            }

            auc.newmethod = auc.newmethod / (R- nem);
            auc.jaccard  = auc.jaccard  /( R- jac);
            auc.preferetial  = auc.preferetial  /( R- pre);
            auc.hdi  = auc.hdi  /( R- hdi);
            auc.hpi  = auc.hpi  /( R- hpi);
            auc.adaminc  = auc.adaminc  / (R- adc);



        }
    }
    class Program
    {
        static void Main(string[] args)
        {

            int mn = 297;
            int mf = 500;
          //  for(int i = 1; i < 10; i ++)
            Console.Beep();
            

            //  graph g = new graph(19023, 10);
            //  g.loadfromfile("ds.txt");

            graph.AucScore a = new graph.AucScore();
              graph g = new graph(mn, mf);
           int  Size =   g.loadfromfileEg("ce.txt");

            graph.Node[] test = new graph.Node[mn];
            for (int i = 0; i < mn ; i++)
                test[i] = new graph.Node(mf );
           int ts =  g.generate(ref test, mn, mf ,Size, 10);
            
            g.AUC(test,ref a, mn, 1000 , ts  );


            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("              AUC  "+ts.ToString());
            Console.WriteLine("NEW METHOD  : " + a.newmethod );
            Console.WriteLine("ADAMIC ADAR : " + a.adaminc );
            Console.WriteLine("JACCARD     : "+ a.jaccard );
            Console.WriteLine("HDI         : "+ a.hdi );
            Console.WriteLine("HPI         : " + a.hpi );
            Console.WriteLine("PREFRETIAL  : " + a.preferetial );
            for (int i = 1; i < 4; i++)
                Console.Beep();



            /*

            foreach (graph.Node n in Test)
                if((n != null) )
                    if (n.freinds[0] != 0) 
                        Console.WriteLine(n.freinds[0].ToString() + "   " + n.freinds[1].ToString());

            Console.WriteLine("generation compelete , enter node index");
            ArrayList jaccardproposal = new ArrayList();
            ArrayList  adamicproposal = new  ArrayList() ;
            ArrayList preferetial = new ArrayList();
            ArrayList HDI = new ArrayList();
            ArrayList HPI = new ArrayList();
            ArrayList newMethod = new ArrayList();
            int  value;

            g.adamicadar(ref adamicproposal,value =   Convert.ToInt32( Console.ReadLine()));
            g.jaccard(ref jaccardproposal, value );
            g.preferentialATCH (ref preferetial , value);
            g.HDI(ref HDI , value);
            g.HPI(ref HPI, value);
            g.newMethod(ref newMethod , value);

            

            Console.WriteLine("NEW METHOD");
            foreach (graph.Node n in newMethod )
                Console.WriteLine(n.freinds[0] + "   " + n.newMethod );
            Console.WriteLine("ADAMIC ADAR");
            foreach (graph.Node n in adamicproposal)
                Console.WriteLine(n.freinds[0] +"   "+ n.score);
            Console.WriteLine();
            Console.WriteLine("JACCARD");
            foreach (graph.Node n in jaccardproposal)
                Console.WriteLine(n.freinds[0] + "   " + n.jaccard );
            Console.WriteLine();
            Console.WriteLine("PREFERETIAL ATTACHMENT");
            foreach (graph.Node n in preferetial )
                Console.WriteLine(n.freinds[0] + "   " + n.preferential );
            Console.WriteLine();
            Console.WriteLine("HDI");
            foreach (graph.Node n in HDI)
                Console.WriteLine(n.freinds[0] + "   " + n.HDI );
            Console.WriteLine();
            Console.WriteLine("HPI");
            foreach (graph.Node n in HPI)
                Console.WriteLine(n.freinds[0] + "   " + n.HPI);


            */

            Console.Read();
            Console.Read();

        }
    }
}
