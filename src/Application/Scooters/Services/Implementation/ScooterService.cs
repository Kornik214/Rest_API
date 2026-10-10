using Application.Common.Interfaces;
using Application.Scooters.Services.Abstract;
using Domain.Scooters;

namespace Application.Scooters.Services.Implementation;

public class ScooterService : IScooterService
{
    private readonly IScooterRepository _scooterRepository;

    public ScooterService(IScooterRepository scooterRepository)
    {
        _scooterRepository = scooterRepository;
    }

    public async Task<IReadOnlyList<Scooter>> GetScooters(
        CancellationToken cancellationToken)
    {
        return await _scooterRepository.GetAll(cancellationToken);
    }

    public async Task<Scooter?> GetScooter(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _scooterRepository.GetById(id, cancellationToken);
    }

    public async Task<Scooter> Add(
        string model,
        string serialNumber,
        int batteryPercent,
        decimal pricePerMinute,
        bool isAvailable,
        DateTime lastServiceDate,
        CancellationToken cancellationToken)
    {
        var existingScooter = await _scooterRepository.GetBySerialNumber(
            serialNumber,
            cancellationToken);

        if (existingScooter is not null)
        {
            throw new ArgumentException(
                $"Scooter with serial number '{serialNumber}' already exists.");
        }

        var scooter = Scooter.New(
            Guid.NewGuid(),
            model,
            serialNumber,
            batteryPercent,
            pricePerMinute,
            isAvailable,
            lastServiceDate);

        return await _scooterRepository.Add(scooter, cancellationToken);
    }

    public async Task<Scooter?> Update(
        Guid id,
        string model,
        string serialNumber,
        int batteryPercent,
        decimal pricePerMinute,
        bool isAvailable,
        DateTime lastServiceDate,
        CancellationToken cancellationToken)
    {
        var scooter = await _scooterRepository.GetById(id, cancellationToken);

        if (scooter is null)
        {
            return null;
        }

        var scooterWithSameSerialNumber =
            await _scooterRepository.GetBySerialNumber(
                serialNumber,
                cancellationToken);

        if (scooterWithSameSerialNumber is not null &&
            scooterWithSameSerialNumber.Id != id)
        {
            throw new ArgumentException(
                $"Scooter with serial number '{serialNumber}' already exists.");
        }

        scooter.UpdateDetails(
            model,
            serialNumber,
            batteryPercent,
            pricePerMinute,
            isAvailable,
            lastServiceDate);

        return await _scooterRepository.Update(scooter, cancellationToken);
    }

    public async Task<bool> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var scooter = await _scooterRepository.GetById(id, cancellationToken);

        if (scooter is null)
        {
            return false;
        }

        await _scooterRepository.Delete(scooter, cancellationToken);

        return true;
    }
}