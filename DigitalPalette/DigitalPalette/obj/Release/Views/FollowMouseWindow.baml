   M S B A M L   `   `   `  ���� I  EDigitalPalette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:DigitalPalette.ViewModelsDigitalPalette.ViewModels  E'clr-namespace:DigitalPalette.ViewModelsDigitalPalette.ViewModels  ,    &DigitalPalette.Views.FollowMouseWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     9local'clr-namespace:DigitalPalette.ViewModels     6VM'clr-namespace:DigitalPalette.ViewModels       � AllowsTransparency  .  � Title$ FollowMouseWindow��$	��120��$	��120�� � Topmost .  � ShowInTaskbar .   � WindowStyle$
 None=� � 
ResizeMode$ NoResize=�
������  � Left �� *left+ �� Mode$ TwoWay=�
 � Top �� *top+$ TwoWay=�.��� $��Center=�$��Center=�.+��� $	��101��$	��101��$��1q�	 �� CornerRadius$	 10������ *nowColor.foreGroundColor+�� $	��100��$	��100��$��10q�$	 10��   BlackShadow1
 J� Effect#
 �   ���� *nowColor.solidColorbrush+._�(� $��2q�.���� 
������ $	��100�� � 	Resources  �� 
TargetType �� Property �� Value��%h   '��       �� " ��.���  � �� �� *nowColor.foreGroundColor+��  � ��$ 
ExtraBlack"�.+���  �� Orientation$ 
Horizontal=�$��Center=�.+��� $
��R : ���� ���� *
nowColor.r+�� $ 
Horizontal=�$��Center=�.+��� $
��G : ���� ���� *
nowColor.g+�� $ 
Horizontal=�$��Center=�.+��� $
��B : ��$��Center=��� $��Center=����� *
nowColor.b+�� $ 
Horizontal=�$��Center=�.+��� $��Hex : #���� ���� *nowColor.hex+