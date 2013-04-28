unit Unit1;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls;

type
  TForm1 = class(TForm)
    Button1: TButton;
    Memo1: TMemo;
    Memo2: TMemo;
    Label1: TLabel;
    Label2: TLabel;
    Button3: TButton;
    Label3: TLabel;
    Edit1: TEdit;
    Label4: TLabel;
    Edit2: TEdit;
    Label5: TLabel;
    Memo3: TMemo;
    Memo4: TMemo;
    Button2: TButton;
    Label6: TLabel;
    Edit3: TEdit;
    procedure Button1Click(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;
  f_s:string;F:TextFile;St1:string;
  data1:array[1..20000]of extended;//1st data
  data2:array[1..20000]of extended;//2nd data
  i,j,i1,i2,n1:LongInt;
  r:array[0..20000]of extended;//covariance function
  m1,m2:extended;//mathematical expectations
  s1,s2:extended;//standard deviation
  max, max0, max1, max2:extended;//maximum absolute covariation
  imax, inp0, inp1, inp2:LongInt;//number of the maximum absolute covariation
  sar:array[1..5000] of record s1:string;v1:extended;end;
  nsar:LongInt;//number of elements in array sar
  ns:LongInt;//Quantity of cities

implementation

procedure TForm1.Button1Click(Sender: TObject);
begin
  f_s:='Cities\' + Edit1.Text;
  AssignFile(F,f_s);reset(F);
  n1:=0;
  while not EOF(F) do begin
    readln(F,St1);
    inc(n1);
    data1[n1]:=StrToFloat(St1);
  end;
  CloseFile(F);//We have read 1st data

  f_s:='Cities\' + Edit2.Text;
  AssignFile(F,f_s);reset(F);
  n1:=0;
  while not EOF(F) do begin
    readln(F,St1);
    inc(n1);
    data2[n1]:=StrToFloat(St1);
  end;
  CloseFile(F);//We have read 2nd data

  Memo1.Clear;Memo1.Lines.Append(IntToStr(n1));
  for i:=0 to (n1-1) do Memo1.Lines.Append(IntToStr(i+1)+'  '+FloatToStr(data1[i+1]));
  Memo3.Clear;Memo3.Lines.Append(IntToStr(n1));
  for i:=0 to (n1-1) do Memo3.Lines.Append(IntToStr(i+1)+'  '+FloatToStr(data2[i+1]));
end;

procedure TForm1.Button3Click(Sender: TObject);
begin
//Initial data's input
  f_s:='Cities\' + Edit1.Text;
  AssignFile(F,f_s);reset(F);
  n1:=0;
  while not EOF(F) do begin
    readln(F,St1);
    inc(n1);
    data1[n1]:=StrToFloat(St1);
  end;
  CloseFile(F);//We have read 1st data

  f_s:='Cities\' + Edit2.Text;
  AssignFile(F,f_s);reset(F);
  n1:=0;
  while not EOF(F) do begin
    readln(F,St1);
    inc(n1);
    data2[n1]:=StrToFloat(St1);
  end;
  CloseFile(F);//We have read 2nd data

  Memo2.Clear; m1:=0; m2:=0; max:=0;
  for i:=1 to n1 do begin
    m1:=m1+data1[i];m2:=m2+data2[i];
  end;
  m1:=m1/n1;//1st mathematical expectation
  m2:=m2/n1;//2nd mathematical expectation
  s1:=0; s2:=0;
  for i:=1 to n1 do begin
    s1:=s1+sqr(data1[i]-m1);s2:=s2+sqr(data2[i]-m2);
  end;
  s1:=sqrt(s1/n1);//1st standard deviation
  s2:=sqrt(s2/n1);//2nd standard deviation
  for i:=0 to 366 do begin
    r[i]:=0;
    for j:=1 to (n1-i) do r[i]:=r[i]+(data1[j+i]-m1)*(data2[j]-m2);
    r[i]:=r[i]/((n1-i)*s1*s2);
    if max < abs(r[i]) then begin max:= abs(r[i]); imax:=i;end;
    Memo2.Lines.Append(IntToStr(i)+'  '+FloatToStr(r[i]));
  end;
  Memo2.Lines.Append('');Memo2.Lines.Append('Maximum absolute covariation = '+FloatToStr(max));
  Memo2.Lines.Append('Number of the maximum absolute covariation = '+IntToStr(imax));
end;

procedure TForm1.Button2Click(Sender: TObject);
begin
  Memo4.Clear; nsar:=0; ns:=StrToInt(Edit3.Text);
  for i1:=1 to ns do begin
    f_s:='Cities\f' + IntToStr(i1) + '.txt';
    AssignFile(F,f_s);reset(F);
    n1:=0;
    while not EOF(F) do begin
      readln(F,St1);
      inc(n1);
      data1[n1]:=StrToFloat(St1);
    end;
    CloseFile(F);//We have read 1st data

    inp0:=0;inp1:=1;inp2:=0;
    max0:=0;max1:=0;max2:=0;
    for i2:=1 to ns do begin
      f_s:='Cities\f' + IntToStr(i2) + '.txt';
      AssignFile(F,f_s);reset(F);
      n1:=0;
      while not EOF(F) do begin
        readln(F,St1);
        inc(n1);
        data2[n1]:=StrToFloat(St1);
      end;
      CloseFile(F);//We have read 2nd data

      //Covariance calculation
      m1:=0; m2:=0;
      for i:=1 to n1 do begin
        m1:=m1+data1[i];m2:=m2+data2[i];
      end;
      m1:=m1/n1;//1st mathematical expectation
      m2:=m2/n1;//2nd mathematical expectation
      s1:=0; s2:=0;
      for i:=1 to n1 do begin
        s1:=s1+sqr(data1[i]-m1);s2:=s2+sqr(data2[i]-m2);
      end;
      s1:=sqrt(s1/n1);//1st standard deviation
      s2:=sqrt(s2/n1);//2nd standard deviation
      max:=0;
      for i:=150 to 200 do begin
        r[i]:=0;
        for j:=1 to (n1-i) do r[i]:=r[i]+(data1[j+i]-m1)*(data2[j]-m2);
        r[i]:=r[i]/((n1-i)*s1*s2);
        if max < abs(r[i]) then begin
           max:= abs(r[i]); imax:=i;
        end;
      end;
      if nsar=0 then begin
        sar[1].s1:= 'MAC between ' + IntToStr(i1) + ' and ' + IntToStr(i2) + ' = ' + FloatToStr(r[imax]) + ' ; number of the MAC = ' + IntToStr(imax);
        sar[1].v1:= max;
      end else begin
        i:=1;
        while (i<=nsar)and(sar[i].v1>max) do inc(i);
        for j:=nsar downto i do begin
          sar[j+1].s1:= sar[j].s1;
          sar[j+1].v1:= sar[j].v1;
        end;
        sar[i].s1:= 'MAC between ' + IntToStr(i1) + ' and ' + IntToStr(i2) + ' = ' +FloatToStr(r[imax]) + ' ; number of the MAC = ' + IntToStr(imax);
        sar[i].v1:= max;
      end;
      inc(nsar);
      Memo4.Lines.Append('MAC between ' + IntToStr(i1) + ' and ' + IntToStr(i2) + ' = ' +FloatToStr(r[imax]) + ' ; number of the MAC = ' + IntToStr(imax));
      if (max0 < abs(r[imax])) then begin
         max0:=abs(r[imax]);inp0:=i2;
      end else
        if (max1 < abs(r[imax])) then begin
          max1:=abs(r[imax]);inp1:=i2;
        end else
          if (max2 < abs(r[imax])) then begin max2:=abs(r[imax]);inp2:=i2;end;
    end;
    Memo4.Lines.Append('1st number ' + IntToStr(i1) + ' and ' + IntToStr(inp0) + ' = ' +FloatToStr(max0));
    Memo4.Lines.Append('2nd number ' + IntToStr(i1) + ' and ' + IntToStr(inp1) + ' = ' +FloatToStr(max1));
    Memo4.Lines.Append('3rd number ' + IntToStr(i1) + ' and ' + IntToStr(inp2) + ' = ' +FloatToStr(max2));
  end;
  Memo4.Lines.Append('');
  for i:=1 to nsar do
    Memo4.Lines.Append(sar[i].s1);
  Memo4.Lines.Append(''); Memo4.Lines.Append('MAC = Maximum Absolute Covariation');
end;

initialization

{$R *.dfm}

end.
