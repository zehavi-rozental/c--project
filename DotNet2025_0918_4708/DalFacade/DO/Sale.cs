
using System;

namespace DO;

public record Sale(int Id,
    int ProductId, 
    int AmmontRequird,
    double TotalPrice 
    ,bool IsClubMembers,
    DateTime StartSale,
    DateTime EndSale)
{
   
}
