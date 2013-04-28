unit Unit1;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls;

type
  TForm1 = class(TForm)
    Memo1: TMemo;
    Button1: TButton;
    Button2: TButton;
    Label1: TLabel;
    Edit1: TEdit;
    Label2: TLabel;
    Edit2: TEdit;
    Label3: TLabel;
    Edit3: TEdit;
    Edit4: TEdit;
    Label4: TLabel;
    Label5: TLabel;
    Edit5: TEdit;
    Label6: TLabel;
    Edit6: TEdit;
    Label7: TLabel;
    Edit7: TEdit;
    Label8: TLabel;
    Edit8: TEdit;
    Label9: TLabel;
    Edit9: TEdit;
    Label10: TLabel;
    Edit10: TEdit;
    Edit11: TEdit;
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;
  f_s:string;F:TextFile;St1:string;
  i,j,jb,n1:LongInt;//n1 - quantity of the data's lines
  date:array[1..20000]of string;
  X:array[-2..40,1..20000]of Extended;//All inital data: X[-2] -forecasting values
  max:array[-1..40]of Extended;//absolute maximum values
  k0,k1,k2,k3:Extended;//coefficients of the model
  k0b,k1b,k2b,k3b:Extended;//best coefficients
  crit:Extended;//criterion
  d1,d2,sm1:Extended;//coefficients of the criterion
  a1:Extended=1;
  a2:Extended=1;
  a3:Extended=10;
  f1:Extended;//temporary variables
  tsk:LongInt;//Time series' quantity
  e1,e2,e3:LongInt;//Time series' exceptions
  h1:Extended;//Step of the k's changes
  dl:LongInt;//Half-Length of the learning sequence
  dl1:LongInt;//Length of the learning sequence - 2
  dlg:LongInt;//Total length of the sequence

implementation

procedure TForm1.Button1Click(Sender: TObject);
begin
  Memo1.Clear;
  f_s:='Cities\date.txt';
  AssignFile(F,f_s);reset(F);
  n1:=0;
  while not EOF(F) do begin
    inc(n1);
    readln(F,date[n1]);
  end;
  CloseFile(F);//We have read dates

  Memo1.Lines.Append(IntToStr(n1));
  tsk:=StrToInt(Edit2.Text);//Time series' quantity
  for j:=-1 to tsk do begin
    f_s:='Cities\data' + IntToStr(j) + '.txt';
    AssignFile(F,f_s);reset(F);
    for i:=1 to n1 do begin
      readln(F,St1);
      X[j,i]:=StrToFloat(St1);
    end;
    Memo1.Lines.Append(IntToStr(j)+' '+IntToStr(i-1));
    CloseFile(F);//-1 - original data
  end;

  for i:=1 to n1 do begin
    f_s:=date[i];
    for j:=-1 to tsk do f_s:=f_s + ' ' + FloatToStr(X[j,i]);
    Memo1.Lines.Append(f_s);
  end;
end;

procedure TForm1.Button2Click(Sender: TObject);
begin
  e1:=StrToInt(Edit3.Text);e2:=StrToInt(Edit4.Text);e3:=StrToInt(Edit11.Text);//Time series' exceptions
  h1:=StrToFloat(Edit7.Text);//Step of the k's changes
  dl:=StrToInt(Edit8.Text);//Half-Length of the learning sequence
  dl1:=dl+dl-2;//Length of the learning sequence - 2
  dlg:=StrToInt(Edit9.Text) - 2;//Total length of the sequence
  tsk:=StrToInt(Edit2.Text);//Time series' quantity
  crit:=StrToFloat(Edit5.Text);k0:=StrToFloat(Edit6.Text);

  f_s:='Cities\date.txt';
  AssignFile(F,f_s);reset(F);
  n1:=0;
  while not EOF(F) do begin
    inc(n1);
    readln(F,date[n1]);
  end;
  CloseFile(F);//We have read dates

  for j:=-1 to tsk do begin
    f_s:='Cities\data' + IntToStr(j) + '.txt';
    AssignFile(F,f_s);reset(F);
    for i:=1 to n1 do begin
      readln(F,St1);
      X[j,i]:=StrToFloat(St1);
    end;
    CloseFile(F);//-1 - original data
  end;

  Memo1.Clear;
  for i:=-1 to tsk do begin
    max[i]:=0;
    for j:=1 to n1 do
      if abs(X[i,j])>max[i] then max[i]:=abs(X[i,j]);
    for j:=1 to n1 do X[i,j]:=X[i,j]/max[i];//normalization
    Memo1.Lines.Append(IntToStr(i) + ' ' + FloatToStr(max[i]));
  end;

  if Edit1.Text='1' then begin
    //1 parameter
    repeat
      k1:=-2;
      repeat
        for j:=0 to tsk do begin
          i:=0;d1:=0;d2:=0;
          repeat
            inc(i);
            X[-2,i]:=k0+k1*X[j,i];
            d1:=d1+abs(X[-2,i]-X[-1,i]);
            inc(i);
            X[-2,i]:=k0+k1*X[j,i];
            d2:=d2+abs(X[-2,i]-X[-1,i]);
          until i>dl1;
          d1:=d1/dl;d2:=d2/dl;
          f1:=a1*d1+a2*d2+a3*abs(d1-d2);
          if crit>f1 then begin
            crit:=f1;k0b:=k0;k1b:=k1;jb:=j;
            Memo1.Lines.Append(FloatToStr(d1) + ' ' + FloatToStr(d2) + ' ' + FloatToStr(crit) + ' ' + FloatToStr(k0b) + ' ' + FloatToStr(k1b) + ' ' + IntToStr(jb));
          end;
        end;
        k1:=k1+h1;
      until k1>2;
      k0:=k0+h1;
      Memo1.Lines.Append(FloatToStr(k0) + ' ' + FloatToStr(crit) +  ' ' + FloatToStr(k0b) +  ' ' + FloatToStr(k1b) +  ' ' + FloatToStr(jb));
    until k0>StrToFloat(Edit10.Text);

    //Calculation on the examination data
    i:=dl1+2;d1:=0;d2:=0;
    repeat
      inc(i);
      X[-2,i]:=k0b+k1b*X[jb,i];
      d1:=d1+abs(X[-2,i]-X[-1,i]);
      inc(i);
      X[-2,i]:=k0b+k1b*X[jb,i];
      d2:=d2+abs(X[-2,i]-X[-1,i]);
    until i>dlg;
  end else if Edit1.Text='2' then begin
    //2 parameters
    repeat
      k1:=-2;
      repeat
        k2:=-2;
        repeat
          for j:=0 to tsk do if j<>e1 then begin
            i:=0;d1:=0;d2:=0;
            repeat
              inc(i);
              X[-2,i]:=k0+k1*X[e1,i]+k2*X[j,i];
              d1:=d1+abs(X[-2,i]-X[-1,i]);
              inc(i);
              X[-2,i]:=k0+k1*X[e1,i]+k2*X[j,i];
              d2:=d2+abs(X[-2,i]-X[-1,i]);
            until i>dl1;
            d1:=d1/dl;d2:=d2/dl;
            f1:=a1*d1+a2*d2+a3*abs(d1-d2);
            if crit>f1 then begin
              crit:=f1;k0b:=k0;k1b:=k1;k2b:=k2;jb:=j;
              Memo1.Lines.Append(FloatToStr(d1) + ' ' + FloatToStr(d2) + ' ' + FloatToStr(crit) + ' ' + FloatToStr(k0b) + ' ' + FloatToStr(k1b) + ' ' + FloatToStr(k2b) + ' ' + IntToStr(jb));
            end;
          end;
          k2:=k2+h1;
        until k2>2;
        k1:=k1+h1;
      until k1>2;
      k0:=k0+h1;
      Memo1.Lines.Append(FloatToStr(k0) + ' ' + FloatToStr(crit) +  ' ' + FloatToStr(k0b) +  ' ' + FloatToStr(k1b) +  ' ' + FloatToStr(k2b) +  ' ' + FloatToStr(jb));
    until k0>StrToFloat(Edit10.Text);

    //Calculation on the examination data
    i:=dl1+2;d1:=0;d2:=0;
    repeat
      inc(i);
      X[-2,i]:=k0b+k1b*X[e1,i]+k2b*X[jb,i];
      d1:=d1+abs(X[-2,i]-X[-1,i]);
      inc(i);
      X[-2,i]:=k0b+k1b*X[e1,i]+k2b*X[jb,i];
      d2:=d2+abs(X[-2,i]-X[-1,i]);
    until i>dlg;
  end else begin
    //3 parameters
    repeat
      k1:=-1.5;
      repeat
        k2:=-1.5;
        repeat
          k3:=-1.5;
          repeat
              j:=e3;
              i:=0;d1:=0;d2:=0;
              repeat
                inc(i);
                X[-2,i]:=k0+k1*X[e1,i]+k2*X[e2,i]+k3*X[j,i];
                d1:=d1+abs(X[-2,i]-X[-1,i]);
                inc(i);
                X[-2,i]:=k0+k1*X[e1,i]+k2*X[e2,i]+k3*X[j,i];
                d2:=d2+abs(X[-2,i]-X[-1,i]);
              until i>dl1;
              d1:=d1/dl;d2:=d2/dl;
              f1:=a1*d1+a2*d2+a3*abs(d1-d2);
              if crit>f1 then begin
                crit:=f1;k0b:=k0;k1b:=k1;k2b:=k2;k3b:=k3;jb:=j;
                Memo1.Lines.Append(FloatToStr(d1) + ' ' + FloatToStr(d2) + ' ' + FloatToStr(crit) + ' ' + FloatToStr(k0b) + ' ' + FloatToStr(k1b) + ' ' + FloatToStr(k2b) + ' ' + FloatToStr(k3b) + ' ' + IntToStr(jb));
              end;
            k3:=k3+h1;
          until k3>1.5;
          k2:=k2+h1;
        until k2>1.5;
        k1:=k1+h1;
        Memo1.Lines.Append(FloatToStr(k0) + ' ' + FloatToStr(k1) + ' ' + FloatToStr(crit) +  ' ' + FloatToStr(k0b) +  ' ' + FloatToStr(k1b) +  ' ' + FloatToStr(k2b) +  ' ' + FloatToStr(k3b) +  ' ' + FloatToStr(jb));
      until k1>1.5;
      k0:=k0+h1;
    until k0>StrToFloat(Edit10.Text);

    //Calculation on the examination data
    i:=dl1+2;d1:=0;d2:=0;
    repeat
      inc(i);
      X[-2,i]:=k0b+k1b*X[e1,i]+k2b*X[e2,i]+k3b*X[jb,i];
      d1:=d1+abs(X[-2,i]-X[-1,i]);
      inc(i);
      X[-2,i]:=k0b+k1b*X[e1,i]+k2b*X[e2,i]+k3b*X[jb,i];
      d2:=d2+abs(X[-2,i]-X[-1,i]);
    until i>dlg;
  end;

//  d1:=d1/259;d2:=d2/259;
  d1:=d1*2/(dlg-dl1);d2:=d2*2/(dlg-dl1);
  crit:=a1*d1+a2*d2+a3*abs(d1-d2);
  Memo1.Lines.Append('');
  Memo1.Lines.Append(FloatToStr(d1) + ' ' + FloatToStr(d2) + ' ' + FloatToStr(crit));
end;

initialization

{$R *.dfm}

end.
