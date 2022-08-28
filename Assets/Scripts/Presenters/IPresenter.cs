public interface IPresenter<T>
{
	T Model { get; }

	void Inject(T model);

	void RemoveModel();
}