   M S B A M L   `   `   `  ����I  EDigitalPalette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:DigitalPalette.ViewModelsDigitalPalette.ViewModels  5   	   E'clr-namespace:DigitalPalette.ViewModelsDigitalPalette.ViewModels  5   	   ,    &DigitalPalette.Views.FollowMouseWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:DigitalPalette.ViewModels     5   	   6VM'clr-namespace:DigitalPalette.ViewModels     5   	     � AllowsTransparency  .  � Title$ FollowMouseWindow��5
   	   $	��120��6$   $	��120��61    � Topmost .  � ShowInTaskbar .   � WindowStyle$
 None=�5   	    � 
ResizeMode$ NoResize=�6   
������  � Left 5	   	   �� *left+ �� Mode$ TwoWay=�
 � Top 6,   �� *top+$ TwoWay=�.��5      � $��Center=�6   $��Center=�6&   .+�5   
   �� $	��101��6   $	��101��6   $��1q�6*   	 �� CornerRadius$	 10��6o   ��6>   �� *nowColor.foreGroundColor+6o   �� 5   
   $	��100��6   $	��100��6   $��10q�6*   $	 10��6f      BlackShadow1
 J� Effect#
 �   6x   ��66   �� *nowColor.solidColorbrush+._�5      (� $��2q�6   .��5      �� 
������ $	��100��66    � 	Resources 5       �� 
TargetType �� Property �� Value��%�   '��       �� 5      " ��6    .�5      ��  � �� 6;   �� *nowColor.foreGroundColor+�� 5       � ��$ 
ExtraBlack"�6;   5      5      .+�5      ��  �� Orientation$ 
Horizontal=�6!   $��Center=�6:   .+�5      �� $
��R : ��6$   �� 5      ��6$   �� *
nowColor.r+5      �� 5      $ 
Horizontal=�6!   $��Center=�6:   .+�5      �� $
��G : ��6$   �� 5      ��6$   �� *
nowColor.g+5      �� 5       $ 
Horizontal=�6!   $��Center=�6:   .+�5!      �� $
��B : ��6$   $��Center=�60   �� 5"      $��Center=�6@   ��6$   �� *
nowColor.b+6@   5#      �� 5$      $ 
Horizontal=�6!   $��Center=�6:   .+�5%      �� $��Hex : #��6$   �� 5&      ��6$   �� *nowColor.hex+5'      5(      5)      5*      5+      5,      