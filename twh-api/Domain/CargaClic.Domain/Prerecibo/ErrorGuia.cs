using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

public class ErrorGuia : Entity
{
    [Key]
    public int id {get;set;}
    public int  iddetalleguia {get;set;}
    public int iderror {get;set;}
 
}
