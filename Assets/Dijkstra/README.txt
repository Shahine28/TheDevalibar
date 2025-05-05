Dijkstra Pathfinding Package
============================

Ce package implémente l'algorithme de Dijkstra pour trouver le chemin le plus court dans un graphe pondéré. 
Il est conçu pour être utilisé dans Unity avec des Nodes, des Edges, et des Contraintes supplémentaires.

-------------------------------------------------------------
Introduction : Algorithme de Dijkstra
-------------------------------------------------------------
L'algorithme de Dijkstra est une méthode pour trouver le chemin le plus court entre deux points dans un graphe pondéré.

Le graphe est composé de :
- Nodes : Représentent les points du graphe, pouvant avoir un temps d'attente.
- Edges : Relient les Nodes entre eux et ont un coût de déplacement (COST).

Exemple :
Imaginez un graphe avec des villes (Nodes) reliées par des routes (Edges). 
Les routes ont un coût (distance ou temps de voyage), et certaines villes nécessitent un temps d'attente. 
L'algorithme détermine le chemin avec le coût total le plus faible pour aller de la ville de départ à la destination.

-------------------------------------------------------------
NodeManager
-------------------------------------------------------------
Le NodeManager est le composant responsable de la gestion des Nodes, des Edges, et des Contraintes.

Fonctionnalités principales :
-----------------------------
1. **Créer un Node :**
   - Sélectionnez le NodeManager dans la hiérarchie.
   - Maintenez Ctrl et cliquez (LMB) sur la scène pour créer un Node à l'emplacement du clic.

2. **Créer une Edge (connexion entre deux Nodes) :**
   - Maintenez Ctrl et cliquez sur deux Nodes consécutifs.

3. **Supprimer un Node :**
   - Sélectionnez un Node et appuyez sur Suppr pour le supprimer.

4. **Définir les StartNode et EndNode :**
   - Maintenez Ctrl et cliquez (RMB) sur un Node :
     - Si aucun StartNode ou EndNode n'est défini :
       - Le premier Node cliqué devient le StartNode (vert).
       - Le second Node cliqué devient le EndNode (rouge).
     - Si un Node est déjà défini comme StartNode ou EndNode, il redevient un Node classique.
     - Un Node ne peut pas devenir un StartNode ou EndNode si ces rôles sont déjà attribués.

5. **Annulation des actions :**
   - Toutes les actions (création, suppression, connexion) sont Undo compatibles.

Ajout de Contraintes :
----------------------
Dans l'Inspector du NodeManager, vous pouvez ajouter des contraintes supplémentaires 
(types supportés : int, float, bool, Vector3). Ces contraintes influencent le calcul des coûts pour le chemin.

- `int` et `float` : La plus petite valeur est utilisée.
- `bool` : La contrainte doit être true pour être prise en compte.
- `Vector3` : La plus petite magnitude est utilisée.

Les contraintes sont additionnées avec le coût de l'Edge pour obtenir le coût total.

-------------------------------------------------------------
DijkstraManager
-------------------------------------------------------------
Le DijkstraManager est le composant responsable de l'exécution de l'algorithme.

Options principales :
---------------------
1. **Prendre en compte le temps d'attente :**
   - Vous pouvez activer ou désactiver la prise en compte du temps d'attente au niveau des Nodes.

2. **Activer/Désactiver les Contraintes :**
   - Choisissez quelles contraintes pour les Edges doivent être prises en compte dans les calculs (true = pris en compte).

Exécution de l'algorithme :
---------------------------
1. Appelez la méthode `FindShortestPath()` du DijkstraManager pour calculer le chemin entre le StartNode et le EndNode.
2. Le chemin calculé est stocké dans une liste et peut être visualisé dans la scène.

-------------------------------------------------------------
DijkstraPathFollower
-------------------------------------------------------------
Pour faire suivre le chemin calculé à un GameObject :

1. Ajoutez le composant `DijkstraPathFollower` à votre GameObject.
2. La méthode `FollowPath()` sera appelée automatiquement au démarrage (ou manuellement si nécessaire).
3. Le GameObject suivra le chemin calculé par le DijkstraManager.

-------------------------------------------------------------
Résumé des Contrôles dans la Scène :
-------------------------------------------------------------
| Action                            | Commande                     |
|-----------------------------------|------------------------------|
| Créer un Node                     | Ctrl + LMB                   |
| Créer une Edge                    | Ctrl + LMB sur deux Nodes    |
| Supprimer un Node                 | Sélection + Suppr            |
| Définir StartNode ou EndNode      | Ctrl + RMB sur un Node       |

-------------------------------------------------------------
Dépendances :
-------------------------------------------------------------
- NodeManager : Gère la structure du graphe.
- DijkstraManager : Effectue les calculs du chemin.
- DijkstraPathFollower : Suit le chemin calculé.

-------------------------------------------------------------
Notes :
-------------------------------------------------------------
Ce package fournit une solution simple et efficace pour la gestion de graphes pondérés avec contraintes dans Unity.
