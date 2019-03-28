﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.Data;

/// <summary>
/// Summary description for Shuffle
/// </summary>
public class Shuffle
{
    public void ShuffleTable(Database db, int urlcode)
    {
        int NameCount = db.QueryValue("SELECT Count(Name) FROM Players WHERE Room_Code = @0", urlcode);

        float Tbl_Amount = db.QueryValue("SELECT Tbl_Amount FROM Room WHERE Room_Code = @0", urlcode);
        float Tbl_Max_Size = db.QueryValue("SELECT Tbl_Max_Size FROM Room WHERE Room_Code = @0", urlcode);
        float Tbl_Min_Size = db.QueryValue("SELECT Tbl_Min_Size FROM Room WHERE Room_Code = @0", urlcode);
        double Tbl_Needed = Convert.ToInt32(Math.Ceiling(NameCount / Tbl_Max_Size));

        double playerspertable = Math.Floor(NameCount / Tbl_Needed);
        int floorplayerspertable = Convert.ToInt32(playerspertable);

        int[] Tbl_Numberarray = new int[NameCount + 1];
        int[] Count_per_Table = new int[Convert.ToInt32(Tbl_Needed)];
        int playernumber = 0;

        for (int Tbl_Neededcount = 0; Tbl_Neededcount < Tbl_Needed; Tbl_Neededcount++)
        {

            for (int Amountplaypertable = 0; Amountplaypertable < floorplayerspertable; Amountplaypertable++)
            {
                Tbl_Numberarray[playernumber] = Tbl_Neededcount + 1;
                Count_per_Table[Tbl_Neededcount]++;
                playernumber++;
            }
        }

        if (playernumber != NameCount)
        {
            int restplayerspertable = NameCount - playernumber;
            while (restplayerspertable > 0)
            {
                for (int i = 1; i < Tbl_Needed - 1; i++)
                {
                    if (Count_per_Table[i - 1] != Tbl_Max_Size)
                    {
                        Tbl_Numberarray[playernumber] = i;
                        playernumber++;
                        restplayerspertable--;
                    }
                }
            }
        }

        Random rnd = new Random();
        Tbl_Numberarray = Tbl_Numberarray.OrderBy(x => rnd.Next()).ToArray();
        int counting = 0;
        foreach (var row in db.Query("SELECT Name FROM Players WHERE Room_Code = @0", urlcode))
        {

            db.Execute("UPDATE Players SET Tbl_nr = @0 WHERE Name = @1", Tbl_Numberarray[counting], row.Name);
            counting++;
        }
    }
}