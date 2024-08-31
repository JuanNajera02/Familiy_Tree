using Business.Core;
using Data;
using Entities.Entities;
using Familiy_Tree.DTOs;
using Family_Tree.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;


namespace Familiy_Tree.Business
{
    public class PersonsBusiness : BaseBusiness
    {
        public PersonsBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        { }

        public async Task<Persons> Add(AddPersonDTO addPersonDTO)
        {

            //valida que la pareja sea valida
            if (addPersonDTO.PartnerId != null)
            {
                var newPartner = uow.PersonsRepository.GetByID(addPersonDTO.PartnerId);
                if (newPartner.PartnerId != null)
                {
                    throw new ArgumentException(message: "La pareja que se desea asignar ya tiene una pareja");
                }
            }

            // Validar que los hijos no tengan una pareja de padres asignada
            if (addPersonDTO.ChildrenIds != null && addPersonDTO.ChildrenIds.Any())
            {
                var existingRelationships = await uow.RelationshipsRepository.GetAllAsync();

                // Obtener los ChildId de las relaciones existentes
                var existingChildrenIds = existingRelationships.Select(r => r.ChildId).ToList();

                // Verificar si alguno de los ChildrenIds ya tiene una relación
                foreach (var childId in addPersonDTO.ChildrenIds)
                {
                    if (existingChildrenIds.Contains(childId))
                    {
                        throw new ArgumentException("Uno de los hijos que se desea asignar ya tiene una pareja de padres.");
                    }
                }
            }


            //ahora registrame a la persona

            var person = new Persons
            {
                PersonName = addPersonDTO.PersonName,
                PartnerId = addPersonDTO.PartnerId
            };

            uow.PersonsRepository.Insert(person);
            await uow.SaveAsync();

            if (addPersonDTO.PartnerId != null)
            {
                var partner = uow.PersonsRepository.GetByID(addPersonDTO.PartnerId);
                if (partner != null)
                {
                    partner.PartnerId = person.Id;
                    uow.PersonsRepository.Update(partner);
                    await uow.SaveAsync();

                    // Buscar hijos existentes del partner
                    var partnerChildren = await uow.RelationshipsRepository
                        .Get(r => r.MotherId == partner.Id || r.FatherId == partner.Id)
                        .AsNoTracking() // Esto evita el tracking de la entidad para prevenir conflictos
                        .ToListAsync();
                    foreach (var relationship in partnerChildren)
                    {
                        // Asignar FatherId o MotherId basado en cuál sea null
                        if (relationship.FatherId == null)
                        {
                            relationship.FatherId = person.Id;
                        }
                        else if (relationship.MotherId == null)
                        {
                            relationship.MotherId = person.Id;
                        }
                        uow.RelationshipsRepository.Update(relationship);
                    }
                    await uow.SaveAsync();
                }
            }
            // Registrar a los hijos
            if (addPersonDTO.ChildrenIds != null && addPersonDTO.ChildrenIds.Any())
            {
                foreach (var childId in addPersonDTO.ChildrenIds)
                {
                    // Evita duplicar relaciones ya existentes
                    var existingRelationship = await uow.RelationshipsRepository
                        .Get(r => r.ChildId == childId)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (existingRelationship == null)
                    {
                        var relationship = new Relationships
                        {
                            ChildId = childId,
                            MotherId = person.Id,
                            FatherId = addPersonDTO.PartnerId // Puede ser null aquí, lo cual es válido
                        };

                        uow.RelationshipsRepository.Insert(relationship);
                    }
                    else
                    {
                        // Actualiza la relación existente
                        existingRelationship.MotherId = person.Id;
                        existingRelationship.FatherId = addPersonDTO.PartnerId;

                        uow.RelationshipsRepository.Update(existingRelationship);
                    }
                    await uow.SaveAsync();
                }
            }

            return person;

        }


        //delete person
        public async Task Delete(int id)
        {
            var person = uow.PersonsRepository.GetByID(id);
            if (person == null)
            {
                throw new ArgumentException("La persona que se desea eliminar no existe.");
            }

            // Eliminar la relación con la pareja
            if (person.PartnerId != null)
            {
                var partner = uow.PersonsRepository.GetByID(person.PartnerId);
                partner.PartnerId = null;
                uow.PersonsRepository.Update(partner);
                await uow.SaveAsync();
            }

            // Actualizar relaciones con los hijos
            var childrenRelationships = await uow.RelationshipsRepository.Get(r => r.FatherId == id || r.MotherId == id).ToListAsync();
            foreach (var relationship in childrenRelationships)
            {
                if (relationship.FatherId == id)
                {
                    relationship.FatherId = null;
                }
                else if (relationship.MotherId == id)
                {
                    relationship.MotherId = null;
                }
                uow.RelationshipsRepository.Update(relationship);
            }
            await uow.SaveAsync();

            // Eliminar la persona
            uow.PersonsRepository.Delete(person);
            await uow.SaveAsync();
        }


        public async Task<Persons> Update(UpdatePersonDTO updatePersonDTO)
        {
            var person = uow.PersonsRepository.GetByID(updatePersonDTO.PersonId);
            if (person == null)
            {
                throw new ArgumentException("La persona que se desea actualizar no existe.");
            }

            bool needsSave = false;

            // Validar y actualizar la pareja
            if (updatePersonDTO.PartnerId != person.PartnerId)
            {
                if (updatePersonDTO.PartnerId != null)
                {
                    var newPartner = uow.PersonsRepository.GetByID(updatePersonDTO.PartnerId);
                    if (newPartner == null)
                    {
                        throw new ArgumentException("La pareja que se desea asignar no existe.");
                    }
                    if (newPartner.PartnerId != null && newPartner.PartnerId != person.Id)
                    {
                        throw new ArgumentException("La pareja que se desea asignar ya tiene una pareja.");
                    }
                }

                // Actualizar la pareja actual si es diferente
                if (person.PartnerId != null)
                {
                    var oldPartner = uow.PersonsRepository.GetByID(person.PartnerId);
                    if (oldPartner != null)
                    {
                        oldPartner.PartnerId = null;
                        uow.PersonsRepository.Update(oldPartner);
                        needsSave = true;
                    }
                }

                // Actualizar la nueva pareja
                if (updatePersonDTO.PartnerId != null)
                {
                    var newPartner = uow.PersonsRepository.GetByID(updatePersonDTO.PartnerId);
                    if (newPartner != null)
                    {
                        newPartner.PartnerId = person.Id;
                        uow.PersonsRepository.Update(newPartner);
                        needsSave = true;
                    }
                }

                person.PartnerId = updatePersonDTO.PartnerId;
                needsSave = true;
            }

            // Actualizar los datos de la persona
            if (updatePersonDTO.PersonName != person.PersonName)
            {
                person.PersonName = updatePersonDTO.PersonName;
                needsSave = true;
            }

            if (needsSave)
            {
                uow.PersonsRepository.Update(person);
                await uow.SaveAsync();
            }

            // Obtener todas las relaciones donde el MotherId o el FatherId coincida con el ID de la persona o la pareja
            var relatedPersonsIds = new HashSet<int> { person.Id };
            if (updatePersonDTO.PartnerId != null)
            {
                relatedPersonsIds.Add(updatePersonDTO.PartnerId.Value);
            }

            var currentChildrenRelationships = await uow.RelationshipsRepository
                .Get(r => relatedPersonsIds.Contains(r.FatherId.Value) || relatedPersonsIds.Contains(r.MotherId.Value))
                .ToListAsync();

            // Obtener los IDs de los hijos a mantener (incluyendo los hijos de la pareja)
            var childrenToKeep = updatePersonDTO.ChildrenIds ?? new List<int>();

            if (updatePersonDTO.PartnerId != null)
            {
                var partnerChildren = await uow.RelationshipsRepository
                    .Get(r => r.FatherId == updatePersonDTO.PartnerId || r.MotherId == updatePersonDTO.PartnerId)
                    .Select(r => r.ChildId)
                    .ToListAsync();

                // Añadir los hijos de la pareja a la lista de hijos a mantener
                childrenToKeep.AddRange(partnerChildren.Except(childrenToKeep));
            }

            // Eliminar relaciones de hijos que ya no están en la lista actualizada (ahora incluye los hijos de la pareja)
            var relationshipsToRemove = currentChildrenRelationships
                .Where(r => !childrenToKeep.Contains(r.ChildId))
                .ToList();

            foreach (var relationship in relationshipsToRemove)
            {
                uow.RelationshipsRepository.Delete(relationship);
                needsSave = true;
            }

            // Insertar nuevas relaciones
            foreach (var childId in childrenToKeep)
            {
                var existingRelationship = await uow.RelationshipsRepository
                    .Get(r => r.ChildId == childId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (existingRelationship == null)
                {
                    var relationship = new Relationships
                    {
                        ChildId = childId,
                        MotherId = person.Id,
                        FatherId = updatePersonDTO.PartnerId
                    };

                    uow.RelationshipsRepository.Insert(relationship);
                    needsSave = true;
                }
                else
                {
                    bool hasChanged = false;
                    if (existingRelationship.MotherId != person.Id)
                    {
                        existingRelationship.MotherId = person.Id;
                        hasChanged = true;
                    }
                    if (existingRelationship.FatherId != updatePersonDTO.PartnerId)
                    {
                        existingRelationship.FatherId = updatePersonDTO.PartnerId;
                        hasChanged = true;
                    }

                    if (hasChanged)
                    {
                        uow.RelationshipsRepository.Update(existingRelationship);
                        needsSave = true;
                    }
                }
            }

            if (needsSave)
            {
                await uow.SaveAsync();
            }

            return person;
        }


        public async Task<IEnumerable<Persons>> GetAllWNotPartner()
        {
            return await uow.PersonsRepository.Get(p => p.PartnerId == null)
                .Include(p => p.Partner)
                .Include(p => p.FatherRelationships)
                .Include(p => p.MotherRelationships)
                .Include(p => p.ChildRelationships)
                .ToListAsync();
        }

        public async Task<IEnumerable<Persons>> GetAllWNotChildren()
        {
            // Obtener los IDs de los hijos
            var childrenIds = await uow.RelationshipsRepository
                .Get(r => r.FatherId != null || r.MotherId != null)
                .Select(r => r.ChildId)
                .ToListAsync();

            // Obtener las personas que no están en la lista de IDs de hijos
            var personsWithoutChildren = await uow.PersonsRepository
                .Get(p => !childrenIds.Contains(p.Id))
                .Include(p => p.Partner)
                .Include(p => p.FatherRelationships)
                .Include(p => p.MotherRelationships)
                .ToListAsync();

            return personsWithoutChildren;
        }

        //muestra todos las persons
        public async Task<IEnumerable<Persons>> GetAll()
        {
            return await uow.PersonsRepository.Get()
                .Include(p => p.Partner)
                .Include(p => p.FatherRelationships)
                .Include(p => p.MotherRelationships)
                .Include(p => p.ChildRelationships)
                .ToListAsync();
        }

        public async Task<IEnumerable<Persons>> GetAllWNotChildrenAndChildren(int id)
        {
            // Obtener los IDs de todos los hijos (directos o indirectos)
            var allChildrenIds = await uow.RelationshipsRepository
                .Get(r => r.FatherId != null || r.MotherId != null)
                .Select(r => r.ChildId)
                .Distinct() // Asegurarse de que los IDs sean únicos
                .ToListAsync();

            // Obtener la persona especificada y su pareja
            var person = await uow.PersonsRepository
                .Get(p => p.Id == id)
                .Include(p => p.Partner)
                .FirstOrDefaultAsync();

            if (person == null)
            {
                // Si no se encuentra la persona, devolver una lista vacía
                return Enumerable.Empty<Persons>();
            }

            // Obtener los IDs de los hijos directos de la persona especificada
            var directChildren = await uow.RelationshipsRepository
                .Get(r => r.FatherId == id || r.MotherId == id)
                .Select(r => r.ChildId)
                .Distinct() // Asegurarse de que los IDs sean únicos
                .ToListAsync();

            // Obtener los detalles de los hijos directos de la persona especificada
            var children = await uow.PersonsRepository
                .Get(p => directChildren.Contains(p.Id))
                .Include(p => p.FatherRelationships)
                .Include(p => p.MotherRelationships)
                .ToListAsync();

            // Obtener los IDs de todos los hijos directos de la pareja
            var partnerId = person.Partner?.Id;
            var partnerChildrenIds = partnerId.HasValue
                ? await uow.RelationshipsRepository
                    .Get(r => r.FatherId == partnerId.Value || r.MotherId == partnerId.Value)
                    .Select(r => r.ChildId)
                    .Distinct() // Asegurarse de que los IDs sean únicos
                    .ToListAsync()
                : new List<int>();

            // Obtener las personas que no están en la lista de IDs de hijos y que no son la persona con el ID especificado
            var personsWithoutChildren = await uow.PersonsRepository
                .Get(p => !allChildrenIds.Contains(p.Id) && p.Id != id)
                .Include(p => p.Partner)
                .Include(p => p.FatherRelationships)
                .Include(p => p.MotherRelationships)
                .ToListAsync();

            // Excluir a la persona especificada y a su pareja si ambos no tienen hijos
            if (!allChildrenIds.Contains(id) && !partnerChildrenIds.Contains(id))
            {
                personsWithoutChildren = personsWithoutChildren
                    .Where(p => p.Id != id && (partnerId == null || p.Id != partnerId))
                    .ToList();
            }



            // Devolver las personas que no son hijos de nadie y los hijos directos de la persona especificada
            return personsWithoutChildren.Concat(children);
        }




    }
}
